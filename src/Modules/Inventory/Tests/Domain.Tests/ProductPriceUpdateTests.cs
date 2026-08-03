using Inventory.Domain.Entities;
using MasterData.Domain.VO;
using Xunit;

namespace Domain.Tests
{
    public sealed class ProductPriceUpdateTests
    {
        private static readonly ProductServiceLineId SamplePslId = ProductServiceLineId.New();
        private static readonly CurrencyId SampleCurrencyId = CurrencyId.New();

        private static Product CreateProduct()
        {
            return Product.Create(
                "20260731-1-1",
                itemNumberByDay: 1,
                "Producto de prueba",
                SamplePslId,
                cost: 100m,
                SampleCurrencyId,
                costExchangeRate: 150.25m,
                price: 150m,
                SampleCurrencyId,
                priceExchangeRate: 150.25m,
                isSalable: true,
                isPurchasable: true,
                isStored: true).Value;
        }

        [Fact]
        public void UpdatePrices_WithValidData_UpdatesCurrentValues()
        {
            var product = CreateProduct();
            var newTaxId = TaxId.New();

            var result = product.UpdatePrices(
                changedByUserId: Guid.NewGuid(),
                newCost: 120m,
                newCostBase: 100m,
                newCostCurrencyId: SampleCurrencyId,
                newCostExchangeRate: 160m,
                newPrice: 180m,
                newPriceBase: 150m,
                newPriceCurrencyId: SampleCurrencyId,
                newPriceExchangeRate: 160m,
                newPrice2: 190m,
                newPrice3: null,
                newPrice4: null,
                newPrice5: null,
                newPurchaseTaxIds: new[] { newTaxId },
                newSaleTaxIds: new[] { newTaxId },
                oldPurchaseTaxRate: 16m,
                newPurchaseTaxRate: 16m,
                oldSaleTaxRate: 16m,
                newSaleTaxRate: 16m,
                newProfitMargin: 25m);

            Assert.False(result.IsError);
            Assert.Equal(120m, product.Cost);
            Assert.Equal(180m, product.Price);
            Assert.Equal(190m, product.Price2);
            Assert.Equal(25m, product.ProfitMargin);
            Assert.Single(product.PurchaseTaxIds);
            Assert.Equal(newTaxId, product.PurchaseTaxIds[0]);
        }

        [Fact]
        public void UpdatePrices_WithNegativeCost_Fails()
        {
            var product = CreateProduct();

            var result = product.UpdatePrices(
                changedByUserId: Guid.NewGuid(),
                newCost: -1m,
                newCostBase: null,
                newCostCurrencyId: null,
                newCostExchangeRate: null,
                newPrice: 100m,
                newPriceBase: null,
                newPriceCurrencyId: null,
                newPriceExchangeRate: null,
                newPrice2: null,
                newPrice3: null,
                newPrice4: null,
                newPrice5: null,
                newPurchaseTaxIds: Array.Empty<TaxId>(),
                newSaleTaxIds: Array.Empty<TaxId>(),
                oldPurchaseTaxRate: null,
                newPurchaseTaxRate: null,
                oldSaleTaxRate: null,
                newSaleTaxRate: null,
                newProfitMargin: null);

            Assert.True(result.IsError);
            Assert.Equal("Product.NegativeCost", result.FirstError.Code);
        }

        [Fact]
        public void UpdatePrices_WithNegativePrice_Fails()
        {
            var product = CreateProduct();

            var result = product.UpdatePrices(
                changedByUserId: Guid.NewGuid(),
                newCost: 100m,
                newCostBase: null,
                newCostCurrencyId: null,
                newCostExchangeRate: null,
                newPrice: -1m,
                newPriceBase: null,
                newPriceCurrencyId: null,
                newPriceExchangeRate: null,
                newPrice2: null,
                newPrice3: null,
                newPrice4: null,
                newPrice5: null,
                newPurchaseTaxIds: Array.Empty<TaxId>(),
                newSaleTaxIds: Array.Empty<TaxId>(),
                oldPurchaseTaxRate: null,
                newPurchaseTaxRate: null,
                oldSaleTaxRate: null,
                newSaleTaxRate: null,
                newProfitMargin: null);

            Assert.True(result.IsError);
            Assert.Equal("Product.NegativePrice", result.FirstError.Code);
        }

        [Fact]
        public void UpdatePrices_RecordsOldAndNewValuesInHistory()
        {
            var product = CreateProduct(); // Cost=100m, Price=150m al crear
            var userId = Guid.NewGuid();

            product.UpdatePrices(
                changedByUserId: userId,
                newCost: 120m,
                newCostBase: 100m,
                newCostCurrencyId: SampleCurrencyId,
                newCostExchangeRate: 160m,
                newPrice: 180m,
                newPriceBase: 150m,
                newPriceCurrencyId: SampleCurrencyId,
                newPriceExchangeRate: 160m,
                newPrice2: null,
                newPrice3: null,
                newPrice4: null,
                newPrice5: null,
                newPurchaseTaxIds: Array.Empty<TaxId>(),
                newSaleTaxIds: Array.Empty<TaxId>(),
                oldPurchaseTaxRate: 16m,
                newPurchaseTaxRate: 8m,
                oldSaleTaxRate: 16m,
                newSaleTaxRate: 8m,
                newProfitMargin: 30m);

            Assert.Single(product.PriceHistory);
            var entry = product.PriceHistory[0];
            Assert.Equal(userId, entry.ChangedByUserId);
            Assert.Equal(100m, entry.OldCost);
            Assert.Equal(120m, entry.NewCost);
            Assert.Equal(150m, entry.OldPrice);
            Assert.Equal(180m, entry.NewPrice);
            Assert.Equal(16m, entry.OldPurchaseTaxRate);
            Assert.Equal(8m, entry.NewPurchaseTaxRate);
        }

        [Fact]
        public void UpdatePrices_CalledTwice_AccumulatesTwoHistoryEntries()
        {
            var product = CreateProduct();

            product.UpdatePrices(
                Guid.NewGuid(), 110m, null, null, null, 160m, null, null, null,
                null, null, null, null,
                Array.Empty<TaxId>(), Array.Empty<TaxId>(),
                null, null, null, null, null);

            product.UpdatePrices(
                Guid.NewGuid(), 120m, null, null, null, 170m, null, null, null,
                null, null, null, null,
                Array.Empty<TaxId>(), Array.Empty<TaxId>(),
                null, null, null, null, null);

            Assert.Equal(2, product.PriceHistory.Count);
        }
    }
}
