using ErrorOr;
using Inventory.Application.Products.Models;
using Inventory.Domain.Repositories;
using Inventory.Domain.VO;
using MediatR;

namespace Inventory.Application.Products.Queries.GetLastPrice
{
    internal sealed class GetProductLastPriceQueryHandler : IRequestHandler<GetProductLastPriceQuery, ErrorOr<ProductLastPriceResult?>>
    {
        private readonly IProductRepository _repository;

        public GetProductLastPriceQueryHandler(IProductRepository repository)
        {
            _repository = repository;
        }

        public async Task<ErrorOr<ProductLastPriceResult?>> Handle(GetProductLastPriceQuery request, CancellationToken cancellationToken)
        {
            var history = await _repository.GetLastPriceHistoryAsync(
                ProductId.Create(request.ProductId), cancellationToken);

            if (history is null)
                return (ProductLastPriceResult?)null;

            return new ProductLastPriceResult(
                history.OldCost,
                history.OldCostCurrencyId?.Value,
                history.OldCostExchangeRate,
                history.OldPrice,
                history.OldPriceCurrencyId?.Value,
                history.OldPriceExchangeRate,
                history.OldPrice2,
                history.OldPrice3,
                history.OldPrice4,
                history.OldPrice5,
                history.ChangedAt);
        }
    }
}
