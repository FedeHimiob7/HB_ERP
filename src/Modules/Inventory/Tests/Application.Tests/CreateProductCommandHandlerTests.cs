using HB_ERP.SharedKernel.Domain;
using HB_ERP.SharedKernel.Domain.Primitives;
using Inventory.Application.Products.Commands.CreateProduct;
using Inventory.Domain.DomainErrors;
using Inventory.Domain.Entities;
using Inventory.Domain.Repositories;
using MasterData.Domain.Entities;
using MasterData.Domain.Repositories;
using MasterData.Domain.VO;
using NSubstitute;
using Xunit;

namespace Application.Tests
{
    public sealed class CreateProductCommandHandlerTests
    {
        private readonly IProductRepository _repository = Substitute.For<IProductRepository>();
        private readonly IProductServiceLineRepository _pslRepository = Substitute.For<IProductServiceLineRepository>();
        private readonly IProductCodeCounterRepository _counterRepository = Substitute.For<IProductCodeCounterRepository>();
        private readonly Inventory.Application.Interfaces.IInventoryUnitOfWork _unitOfWork = Substitute.For<Inventory.Application.Interfaces.IInventoryUnitOfWork>();
        private readonly ICurrentUserProvider _currentUser = Substitute.For<ICurrentUserProvider>();
        private readonly Guid _pslGuid = Guid.NewGuid();

        private CreateProductCommandHandler CreateHandler()
            => new(_repository, _pslRepository, _counterRepository, _unitOfWork, _currentUser);

        private CreateProductCommand CreateCommand() => new(
            Code: "20260810-1-1-1",
            Name: "Zapato deportivo",
            ProductServiceLineId: _pslGuid,
            Description: null, Model: null, Barcode: null, ClientCode: null,
            ProductTypeId: null, ProductCategoryId: null, ProductSubCategoryId: null, ProductBrandId: null,
            IsSalable: true, IsPurchasable: true, IsStored: true,
            Cost: null, CostCurrencyId: null, CostExchangeRate: null,
            Price: null, PriceCurrencyId: null, PriceExchangeRate: null,
            Price2: null, Price3: null, Price4: null, Price5: null,
            PurchaseTaxIds: new List<Guid>(), SaleTaxIds: new List<Guid>(),
            PurchaseUnitId: null, SaleUnitId: null, UnitConversionFactor: null,
            Weight: null, Volume: null, ContentCapacity: null,
            Tags: null, ImageUrl: null, ProfitMargin: null);

        [Fact]
        public async Task Handle_WhenPslNotInCurrentUserPsls_ReturnsPslAccessDenied()
        {
            _currentUser.PslIds.Returns(new List<Guid> { Guid.NewGuid() });

            var result = await CreateHandler().Handle(CreateCommand(), CancellationToken.None);

            Assert.True(result.IsError);
            Assert.Equal(CommonErrors.PslAccessDenied.Code, result.FirstError.Code);
        }

        [Fact]
        public async Task Handle_WhenCodeAlreadyExists_ReturnsDuplicateCode()
        {
            _currentUser.PslIds.Returns(new List<Guid> { _pslGuid });
            _pslRepository.GetByIdAsync(Arg.Any<ProductServiceLineId>(), Arg.Any<CancellationToken>())
                .Returns(ProductServiceLine.CreateExisting(_pslGuid, "desc", "Calzado", isActive: true));
            _repository.ExistsByCodeAsync(Arg.Any<string>(), excludeId: Arg.Any<Inventory.Domain.VO.ProductId?>(), cancellationToken: Arg.Any<CancellationToken>())
                .Returns(true);

            var result = await CreateHandler().Handle(CreateCommand(), CancellationToken.None);

            Assert.True(result.IsError);
            Assert.Equal(ProductErrors.DuplicateCode.Code, result.FirstError.Code);
        }

        [Fact]
        public async Task Handle_WhenValid_ConsumesReservationAndPersists()
        {
            _currentUser.PslIds.Returns(new List<Guid> { _pslGuid });
            _pslRepository.GetByIdAsync(Arg.Any<ProductServiceLineId>(), Arg.Any<CancellationToken>())
                .Returns(ProductServiceLine.CreateExisting(_pslGuid, "desc", "Calzado", isActive: true));
            _repository.ExistsByCodeAsync(Arg.Any<string>(), excludeId: Arg.Any<Inventory.Domain.VO.ProductId?>(), cancellationToken: Arg.Any<CancellationToken>())
                .Returns(false);
            // Caso normal: el código no fue editado por el usuario, la reserva de GenerateProductCode
            // sigue vigente y ConsumeAsync la encuentra directamente (no hace falta el fallback).
            _counterRepository.ConsumeAsync(Arg.Any<string>(), Arg.Any<ProductServiceLineId>(), Arg.Any<CancellationToken>())
                .Returns(3);

            var result = await CreateHandler().Handle(CreateCommand(), CancellationToken.None);

            Assert.False(result.IsError);
            await _repository.Received(1).AddAsync(
                Arg.Is<Product>(p => p.Code == "20260810-1-1-1" && p.ItemNumberByDay == 3),
                Arg.Any<CancellationToken>());
            await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_WhenUserEditedCode_FallsBackToReservingNewCounter()
        {
            // Si el usuario editó el código antes de enviarlo, ConsumeAsync no encuentra la reserva
            // original (devuelve null) — el handler debe reservar un correlativo nuevo con ese código.
            _currentUser.PslIds.Returns(new List<Guid> { _pslGuid });
            _pslRepository.GetByIdAsync(Arg.Any<ProductServiceLineId>(), Arg.Any<CancellationToken>())
                .Returns(ProductServiceLine.CreateExisting(_pslGuid, "desc", "Calzado", isActive: true));
            _repository.ExistsByCodeAsync(Arg.Any<string>(), excludeId: Arg.Any<Inventory.Domain.VO.ProductId?>(), cancellationToken: Arg.Any<CancellationToken>())
                .Returns(false);
            _counterRepository.ConsumeAsync(Arg.Any<string>(), Arg.Any<ProductServiceLineId>(), Arg.Any<CancellationToken>())
                .Returns((int?)null, 7);
            _counterRepository.ReserveNextAsync(Arg.Any<ProductServiceLineId>(), Arg.Any<DateOnly>(), Arg.Any<Func<int, int, string>>(), Arg.Any<CancellationToken>())
                .Returns((1, 7, "codigo-editado"));

            var result = await CreateHandler().Handle(CreateCommand(), CancellationToken.None);

            Assert.False(result.IsError);
            await _repository.Received(1).AddAsync(
                Arg.Is<Product>(p => p.ItemNumberByDay == 7),
                Arg.Any<CancellationToken>());
        }
    }
}
