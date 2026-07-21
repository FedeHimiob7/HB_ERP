using ErrorOr;
using HB_ERP.SharedKernel.Domain;
using HB_ERP.SharedKernel.Domain.Primitives;
using Inventory.Application.Products.Models;
using Inventory.Domain.DomainErrors;
using Inventory.Domain.Repositories;
using MasterData.Domain.Repositories;
using MasterData.Domain.VO;
using MediatR;

namespace Inventory.Application.Products.Commands.GenerateProductCode
{
    internal sealed class GenerateProductCodeCommandHandler : IRequestHandler<GenerateProductCodeCommand, ErrorOr<GenerateProductCodeResult>>
    {
        private readonly IProductCodeCounterRepository _counterRepository;
        private readonly IProductServiceLineRepository _pslRepository;
        private readonly ICurrentUserProvider _currentUser;

        public GenerateProductCodeCommandHandler(
            IProductCodeCounterRepository counterRepository,
            IProductServiceLineRepository pslRepository,
            ICurrentUserProvider currentUser)
        {
            _counterRepository = counterRepository;
            _pslRepository = pslRepository;
            _currentUser = currentUser;
        }

        public async Task<ErrorOr<GenerateProductCodeResult>> Handle(GenerateProductCodeCommand request, CancellationToken cancellationToken)
        {
            if (!_currentUser.PslIds.Contains(request.ProductServiceLineId))
                return CommonErrors.PslAccessDenied;

            var pslId = ProductServiceLineId.Create(request.ProductServiceLineId);

            if (await _pslRepository.GetByIdAsync(pslId, cancellationToken) is null)
                return ProductErrors.InvalidProductServiceLine;

            var today = DateOnly.FromDateTime(DateTime.Now);

            var (_, _, code) = await _counterRepository.ReserveNextAsync(
                pslId,
                today,
                (pslSeq, counter) => $"{today.Year}{today.Month}{today.Day}-{pslSeq}-{counter}",
                cancellationToken);

            return new GenerateProductCodeResult(code);
        }
    }
}
