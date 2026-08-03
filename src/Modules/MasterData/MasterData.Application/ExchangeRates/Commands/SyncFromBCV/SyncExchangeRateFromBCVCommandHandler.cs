using ErrorOr;
using HB_ERP.SharedKernel.Domain.Primitives;
using MasterData.Application.Interfaces;
using MasterData.Domain.DomainErrors;
using MasterData.Domain.Entities;
using MasterData.Domain.Enums;
using MasterData.Domain.Repositories;
using MediatR;

namespace MasterData.Application.ExchangeRates.Commands.SyncFromBCV
{
    internal sealed class SyncExchangeRateFromBCVCommandHandler : IRequestHandler<SyncExchangeRateFromBCVCommand, ErrorOr<ExchangeRateSyncResult>>
    {
        private readonly IBCVRateScrapingService _scrapingService;
        private readonly IExchangeRateRepository _repository;
        private readonly IMasterDataUnitOfWork _unitOfWork;
        private readonly IFiscalClock _fiscalClock;

        public SyncExchangeRateFromBCVCommandHandler(
            IBCVRateScrapingService scrapingService,
            IExchangeRateRepository repository,
            IMasterDataUnitOfWork unitOfWork,
            IFiscalClock fiscalClock)
        {
            _scrapingService = scrapingService;
            _repository = repository;
            _unitOfWork = unitOfWork;
            _fiscalClock = fiscalClock;
        }

        public async Task<ErrorOr<ExchangeRateSyncResult>> Handle(SyncExchangeRateFromBCVCommand request, CancellationToken cancellationToken)
        {
            var fetchedRate = await _scrapingService.GetRateAsync(cancellationToken);
            if (fetchedRate <= 0)
                return ExchangeRateErrors.NoRateAvailable;

            // Si la última tasa registrada tiene el mismo valor, no duplicamos
            var latest = await _repository.GetLatestAsync(cancellationToken);
            if (latest is not null && latest.Rate == fetchedRate)
                return new ExchangeRateSyncResult(latest.Id.Value, latest.Rate, WasCreated: false);

            var result = ExchangeRate.Create(fetchedRate, ExchangeRateSource.BCV, _fiscalClock.VenezuelaNow);
            if (result.IsError) return result.Errors;

            await _repository.AddAsync(result.Value, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new ExchangeRateSyncResult(result.Value.Id.Value, result.Value.Rate, WasCreated: true);
        }
    }
}
