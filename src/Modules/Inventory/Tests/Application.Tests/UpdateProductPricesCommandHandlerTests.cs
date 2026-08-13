using HB_ERP.SharedKernel.Domain.Primitives;
using Inventory.Application.Products.Commands.UpdateProductPrices;
using Inventory.Domain.DomainErrors;
using Inventory.Domain.Entities;
using Inventory.Domain.Repositories;
using Inventory.Domain.VO;
using MasterData.Domain.Entities;
using MasterData.Domain.Repositories;
using MasterData.Domain.VO;
using NSubstitute;
using Xunit;

namespace Application.Tests
{
    public sealed class UpdateProductPricesCommandHandlerTests
    {
        private readonly IProductRepository _repository = Substitute.For<IProductRepository>();
        private readonly IFiscalTaxRateRepository _fiscalTaxRateRepository = Substitute.For<IFiscalTaxRateRepository>();
        private readonly Inventory.Application.Interfaces.IInventoryUnitOfWork _unitOfWork = Substitute.For<Inventory.Application.Interfaces.IInventoryUnitOfWork>();
        private readonly ICurrentUserProvider _currentUser = Substitute.For<ICurrentUserProvider>();
        private readonly IFiscalClock _fiscalClock = Substitute.For<IFiscalClock>();
        private readonly Guid _productGuid = Guid.NewGuid();

        private UpdateProductPricesCommandHandler CreateHandler()
            => new(_repository, _fiscalTaxRateRepository, _unitOfWork, _currentUser, _fiscalClock);

        private static Product CreateSampleProduct() => Product.Create(
            "20260810-1-1-1", 1, "Zapato deportivo", ProductServiceLineId.New(),
            null, null, null, null, null, null, true, true, true).Value;

        [Fact]
        public async Task Handle_WhenNotFoundOrNoPslAccess_ReturnsNotFound()
        {
            _repository.GetByIdAsync(Arg.Any<ProductId>(), Arg.Any<IReadOnlyList<Guid>>(), Arg.Any<CancellationToken>())
                .Returns((Product?)null);

            var command = new UpdateProductPricesCommand(_productGuid, 10m, 8m, null, null, 20m, 16m, null, null, null, null, null, null, null, null, null);
            var result = await CreateHandler().Handle(command, CancellationToken.None);

            Assert.True(result.IsError);
            Assert.Equal(ProductErrors.NotFound.Code, result.FirstError.Code);
        }

        [Fact]
        public async Task Handle_WhenValid_SumsEffectiveTaxRatesForHistoryAndUpdatesPrices()
        {
            var product = CreateSampleProduct();
            _repository.GetByIdAsync(Arg.Any<ProductId>(), Arg.Any<IReadOnlyList<Guid>>(), Arg.Any<CancellationToken>()).Returns(product);

            var ivaTaxId = Guid.NewGuid();
            var ivaRate = FiscalTaxRate.Create(TaxId.Create(ivaTaxId), 0.16m, DateTime.UtcNow).Value;
            _fiscalTaxRateRepository.GetEffectiveManyAsync(Arg.Any<IEnumerable<TaxId>>(), Arg.Any<DateOnly>(), Arg.Any<CancellationToken>())
                .Returns(new Dictionary<TaxId, FiscalTaxRate> { [TaxId.Create(ivaTaxId)] = ivaRate });

            var actingUserGuid = Guid.NewGuid();
            _currentUser.UserId.Returns(actingUserGuid.ToString());

            var command = new UpdateProductPricesCommand(
                _productGuid, 10m, 8m, null, null, 20m, 16m, null, null, null, null, null, null,
                PurchaseTaxIds: new List<Guid> { ivaTaxId },
                SaleTaxIds: new List<Guid> { ivaTaxId },
                ProfitMargin: 0.5m);
            var result = await CreateHandler().Handle(command, CancellationToken.None);

            Assert.False(result.IsError);
            Assert.Equal(10m, result.Value.Cost);
            Assert.Equal(20m, result.Value.Price);
            // La tasa vigente de IVA (0.16) quedó registrada tanto para compra como para venta,
            // porque el mismo TaxId estaba en ambas listas del request.
            Assert.Single(product.PriceHistory);
            Assert.Equal(0.16m, product.PriceHistory[0].NewPurchaseTaxRate);
            Assert.Equal(0.16m, product.PriceHistory[0].NewSaleTaxRate);

            await _repository.Received(1).UpdateAsync(product, Arg.Any<CancellationToken>());
            await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }
    }
}
