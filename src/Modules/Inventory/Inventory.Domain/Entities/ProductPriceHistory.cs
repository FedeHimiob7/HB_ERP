using Inventory.Domain.VO;
using MasterData.Domain.VO;

namespace Inventory.Domain.Entities
{
    public sealed class ProductPriceHistory
    {
        private ProductPriceHistory() { }

        internal ProductPriceHistory(
            ProductId productId,
            Guid changedByUserId,
            decimal? oldCost,
            CurrencyId? oldCostCurrencyId,
            decimal? oldCostExchangeRate,
            decimal? newCost,
            CurrencyId? newCostCurrencyId,
            decimal? newCostExchangeRate,
            decimal? oldPrice,
            CurrencyId? oldPriceCurrencyId,
            decimal? oldPriceExchangeRate,
            decimal? newPrice,
            CurrencyId? newPriceCurrencyId,
            decimal? newPriceExchangeRate,
            decimal? oldPrice2, decimal? newPrice2,
            decimal? oldPrice3, decimal? newPrice3,
            decimal? oldPrice4, decimal? newPrice4,
            decimal? oldPrice5, decimal? newPrice5)
        {
            Id = Guid.NewGuid();
            ProductId = productId;
            ChangedAt = DateTime.UtcNow;
            ChangedByUserId = changedByUserId;

            OldCost = oldCost;
            OldCostCurrencyId = oldCostCurrencyId;
            OldCostExchangeRate = oldCostExchangeRate;
            NewCost = newCost;
            NewCostCurrencyId = newCostCurrencyId;
            NewCostExchangeRate = newCostExchangeRate;

            OldPrice = oldPrice;
            OldPriceCurrencyId = oldPriceCurrencyId;
            OldPriceExchangeRate = oldPriceExchangeRate;
            NewPrice = newPrice;
            NewPriceCurrencyId = newPriceCurrencyId;
            NewPriceExchangeRate = newPriceExchangeRate;

            OldPrice2 = oldPrice2; NewPrice2 = newPrice2;
            OldPrice3 = oldPrice3; NewPrice3 = newPrice3;
            OldPrice4 = oldPrice4; NewPrice4 = newPrice4;
            OldPrice5 = oldPrice5; NewPrice5 = newPrice5;
        }

        public Guid Id { get; private set; }
        public ProductId ProductId { get; private set; }
        public DateTime ChangedAt { get; private set; }
        public Guid ChangedByUserId { get; private set; }

        public decimal? OldCost { get; private set; }
        public CurrencyId? OldCostCurrencyId { get; private set; }
        public decimal? OldCostExchangeRate { get; private set; }
        public decimal? NewCost { get; private set; }
        public CurrencyId? NewCostCurrencyId { get; private set; }
        public decimal? NewCostExchangeRate { get; private set; }

        public decimal? OldPrice { get; private set; }
        public CurrencyId? OldPriceCurrencyId { get; private set; }
        public decimal? OldPriceExchangeRate { get; private set; }
        public decimal? NewPrice { get; private set; }
        public CurrencyId? NewPriceCurrencyId { get; private set; }
        public decimal? NewPriceExchangeRate { get; private set; }

        public decimal? OldPrice2 { get; private set; }
        public decimal? NewPrice2 { get; private set; }
        public decimal? OldPrice3 { get; private set; }
        public decimal? NewPrice3 { get; private set; }
        public decimal? OldPrice4 { get; private set; }
        public decimal? NewPrice4 { get; private set; }
        public decimal? OldPrice5 { get; private set; }
        public decimal? NewPrice5 { get; private set; }
    }
}
