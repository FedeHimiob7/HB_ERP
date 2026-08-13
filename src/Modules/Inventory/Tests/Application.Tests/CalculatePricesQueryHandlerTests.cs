using Inventory.Application.Products.Queries.CalculatePrices;
using Xunit;

namespace Application.Tests
{
    // Motor de cálculo puro (sin dependencias que mockear) — cubre la regla fiscal documentada en
    // CLAUDE.md: "El IGTF se aplica de forma compuesta, sobre el monto que ya incluye los impuestos
    // regulares (no IGTF), no sobre el monto base".
    public sealed class CalculatePricesQueryHandlerTests
    {
        private readonly CalculatePricesQueryHandler _handler = new();

        [Fact]
        public async Task Handle_ForCost_WithNoTaxes_ReturnsBaseAmountUnchanged()
        {
            var query = new CalculatePricesQuery(100m, new List<TaxItemQuery>(), Profit: null, Commission: null, IsCost: true);

            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.False(result.IsError);
            Assert.Equal(100m, result.Value.Cost);
            Assert.Equal(100m, result.Value.CostBase);
        }

        [Fact]
        public async Task Handle_ForCost_WithOnlyRegularTax_AppliesTaxOverBase()
        {
            var taxes = new List<TaxItemQuery> { new(Guid.NewGuid(), 0.16m, IsIGTF: false) };
            var query = new CalculatePricesQuery(100m, taxes, Profit: null, Commission: null, IsCost: true);

            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.False(result.IsError);
            Assert.Equal(116m, result.Value.Cost);
        }

        [Fact]
        public async Task Handle_ForCost_WithRegularTaxAndIGTF_AppliesIGTFOverAmountThatAlreadyIncludesRegularTax()
        {
            // 100 + 16% IVA = 116. Luego el 3% de IGTF se aplica SOBRE 116 (compuesto), no sobre 100.
            var taxes = new List<TaxItemQuery>
            {
                new(Guid.NewGuid(), 0.16m, IsIGTF: false),
                new(Guid.NewGuid(), 0.03m, IsIGTF: true),
            };
            var query = new CalculatePricesQuery(100m, taxes, Profit: null, Commission: null, IsCost: true);

            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.False(result.IsError);
            Assert.Equal(119.48m, result.Value.Cost);
        }

        [Fact]
        public async Task Handle_ForPrice_WithNoTaxesNoProfitNoCommission_Price1ThroughPrice4EqualBaseAndPrice5IsRawBase()
        {
            var query = new CalculatePricesQuery(100m, new List<TaxItemQuery>(), Profit: 0, Commission: null, IsCost: false);

            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.False(result.IsError);
            Assert.Equal(100m, result.Value.Price1);
            // Price5 es el único que se deja en el monto base crudo, sin redondear ni aplicar taxes —
            // es la referencia "sin margen" documentada en el diseño de Product.
            Assert.Equal(100m, result.Value.Price5);
        }

        [Fact]
        public async Task Handle_ForPrice_AppliesFixedProfitMarginsToPrice2ThroughPrice4RegardlessOfRequestedProfit()
        {
            // Price2/3/4 usan márgenes FIJOS del motor (20%/10%/5%), no el Profit que llega en el
            // request — solo Price1 usa el Profit solicitado.
            var query = new CalculatePricesQuery(100m, new List<TaxItemQuery>(), Profit: 50, Commission: null, IsCost: false);

            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.False(result.IsError);
            Assert.Equal(150m, result.Value.Price1);
            Assert.Equal(120m, result.Value.Price2);
            Assert.Equal(110m, result.Value.Price3);
            Assert.Equal(105m, result.Value.Price4);
        }

        [Fact]
        public async Task Handle_ForPrice_AddsCommissionBeforeApplyingTaxes()
        {
            var taxes = new List<TaxItemQuery> { new(Guid.NewGuid(), 0.16m, IsIGTF: false) };
            var query = new CalculatePricesQuery(100m, taxes, Profit: 0, Commission: 10m, IsCost: false);

            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.False(result.IsError);
            // Base = 100 + 0 (profit) + 10 (comisión) = 110; con 16% de IVA = 127.60.
            Assert.Equal(127.60m, result.Value.Price1);
        }
    }
}
