using Inventory.Application.Products.Queries.GetLastPrice;
using Inventory.Domain.Entities;
using Inventory.Domain.Repositories;
using Inventory.Domain.VO;
using MasterData.Domain.VO;
using NSubstitute;
using Xunit;

namespace Application.Tests
{
    public sealed class GetProductLastPriceQueryHandlerTests
    {
        private readonly IProductRepository _repository = Substitute.For<IProductRepository>();
        private readonly Guid _productGuid = Guid.NewGuid();

        private GetProductLastPriceQueryHandler CreateHandler() => new(_repository);

        [Fact]
        public async Task Handle_WhenNoHistoryExists_ReturnsNull()
        {
            _repository.GetLastPriceHistoryAsync(Arg.Any<ProductId>(), Arg.Any<CancellationToken>())
                .Returns((ProductPriceHistory?)null);

            var result = await CreateHandler().Handle(new GetProductLastPriceQuery(_productGuid), CancellationToken.None);

            Assert.False(result.IsError);
            Assert.Null(result.Value);
        }

        [Fact]
        public async Task Handle_WhenHistoryExists_ReturnsOldValuesAsLastPrice()
        {
            // ProductPriceHistory tiene constructor internal — se genera vía la API pública
            // (Product.UpdatePrices), no se instancia a mano.
            var product = Product.Create(
                "20260810-1-1-1", 1, "Zapato deportivo", ProductServiceLineId.New(),
                cost: 10m, costCurrencyId: null, costExchangeRate: null,
                price: 20m, priceCurrencyId: null, priceExchangeRate: null,
                isSalable: true, isPurchasable: true, isStored: true).Value;

            product.UpdatePrices(
                Guid.NewGuid(),
                newCost: 12m, newCostBase: null, newCostCurrencyId: null, newCostExchangeRate: null,
                newPrice: 24m, newPriceBase: null, newPriceCurrencyId: null, newPriceExchangeRate: null,
                newPrice2: null, newPrice3: null, newPrice4: null, newPrice5: null,
                newPurchaseTaxIds: new List<TaxId>(), newSaleTaxIds: new List<TaxId>(),
                oldPurchaseTaxRate: null, newPurchaseTaxRate: null,
                oldSaleTaxRate: null, newSaleTaxRate: null,
                newProfitMargin: null);

            var history = product.PriceHistory[0];
            _repository.GetLastPriceHistoryAsync(Arg.Any<ProductId>(), Arg.Any<CancellationToken>()).Returns(history);

            var result = await CreateHandler().Handle(new GetProductLastPriceQuery(_productGuid), CancellationToken.None);

            Assert.False(result.IsError);
            Assert.NotNull(result.Value);
            // El resultado expone los valores VIEJOS (los que tenía antes del último cambio),
            // no los nuevos — por eso el nombre "LastPrice" se refiere al precio anterior al vigente.
            Assert.Equal(10m, result.Value!.OldCost);
            Assert.Equal(20m, result.Value.OldPrice);
        }
    }
}
