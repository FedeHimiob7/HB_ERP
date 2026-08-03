using ErrorOr;
using HB_ERP.SharedKernel.Domain.Primitives;
using MasterData.Application.ExchangeRates.Models;
using MasterData.Application.Interfaces;
using MasterData.Domain.DomainErrors;
using MasterData.Domain.Entities;
using MasterData.Domain.Enums;
using MasterData.Domain.Repositories;
using MediatR;

namespace MasterData.Application.ExchangeRates.Queries.GetCurrent
{
    internal sealed class GetCurrentExchangeRateQueryHandler : IRequestHandler<GetCurrentExchangeRateQuery, ErrorOr<ExchangeRateResponse>>
    {
        private readonly IExchangeRateRepository _repository;
        private readonly IBCVRateScrapingService _scrapingService;
        private readonly IMasterDataUnitOfWork _unitOfWork;
        private readonly IFiscalClock _fiscalClock;

        public GetCurrentExchangeRateQueryHandler(
            IExchangeRateRepository repository,
            IBCVRateScrapingService scrapingService,
            IMasterDataUnitOfWork unitOfWork,
            IFiscalClock fiscalClock)
        {
            _repository = repository;
            _scrapingService = scrapingService;
            _unitOfWork = unitOfWork;
            _fiscalClock = fiscalClock;
        }

        public async Task<ErrorOr<ExchangeRateResponse>> Handle(GetCurrentExchangeRateQuery request, CancellationToken cancellationToken)
        {
            var fetchedRate = await _scrapingService.GetRateAsync(cancellationToken);

            if (fetchedRate > 0)
            {
                var latest = await _repository.GetLatestAsync(cancellationToken);

                if (latest is null || latest.Rate != fetchedRate)
                {
                    var createResult = ExchangeRate.Create(fetchedRate, ExchangeRateSource.BCV, _fiscalClock.VenezuelaNow);
                    if (createResult.IsError) return createResult.Errors;

                    await _repository.AddAsync(createResult.Value, cancellationToken);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                    latest = createResult.Value;
                }

                return ToResponse(latest);
            }

            // BCV no disponible — devolver la última tasa guardada como fallback
            var fallback = await _repository.GetLatestAsync(cancellationToken);
            if (fallback is null)
                return ExchangeRateErrors.NoRateAvailable;

            return ToResponse(fallback);
        }

        private static ExchangeRateResponse ToResponse(ExchangeRate rate)
            => new(rate.Id.Value, rate.RegisterDate, rate.Rate, rate.Source, rate.Source.ToString());
    }
}
