using ErrorOr;
using HB_ERP.SharedKernel.Domain.Primitives;
using Inventory.Application.Interfaces;
using Inventory.Application.Products.Models;
using Inventory.Domain.DomainErrors;
using Inventory.Domain.Repositories;
using Inventory.Domain.VO;
using MasterData.Domain.VO;
using MediatR;

namespace Inventory.Application.Products.Commands.UpdateProductPrices
{
    internal sealed class UpdateProductPricesCommandHandler : IRequestHandler<UpdateProductPricesCommand, ErrorOr<ProductResponse>>
    {
        private readonly IProductRepository _repository;
        private readonly IInventoryUnitOfWork _unitOfWork;
        private readonly ICurrentUserProvider _currentUser;

        public UpdateProductPricesCommandHandler(
            IProductRepository repository,
            IInventoryUnitOfWork unitOfWork,
            ICurrentUserProvider currentUser)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        public async Task<ErrorOr<ProductResponse>> Handle(UpdateProductPricesCommand request, CancellationToken cancellationToken)
        {
            var product = await _repository.GetByIdAsync(ProductId.Create(request.Id), _currentUser.PslIds, cancellationToken);
            if (product is null) return ProductErrors.NotFound;

            var changedByUserId = Guid.TryParse(_currentUser.UserId, out var uid) ? uid : Guid.Empty;

            var result = product.UpdatePrices(
                changedByUserId,
                request.Cost,
                request.CostCurrencyId is Guid ccid ? CurrencyId.Create(ccid) : null,
                request.CostExchangeRate,
                request.Price,
                request.PriceCurrencyId is Guid pcid ? CurrencyId.Create(pcid) : null,
                request.PriceExchangeRate,
                request.Price2,
                request.Price3,
                request.Price4,
                request.Price5);

            if (result.IsError) return result.Errors;

            await _repository.UpdateAsync(product, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return ProductMapper.ToResponse(product);
        }
    }
}
