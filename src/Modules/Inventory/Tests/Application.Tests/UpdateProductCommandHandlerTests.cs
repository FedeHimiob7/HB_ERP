using HB_ERP.SharedKernel.Domain.Primitives;
using Inventory.Application.Products.Commands.UpdateProduct;
using Inventory.Domain.DomainErrors;
using Inventory.Domain.Entities;
using Inventory.Domain.Repositories;
using Inventory.Domain.VO;
using MasterData.Domain.VO;
using NSubstitute;
using Xunit;

namespace Application.Tests
{
    public sealed class UpdateProductCommandHandlerTests
    {
        private readonly IProductRepository _repository = Substitute.For<IProductRepository>();
        private readonly Inventory.Application.Interfaces.IInventoryUnitOfWork _unitOfWork = Substitute.For<Inventory.Application.Interfaces.IInventoryUnitOfWork>();
        private readonly ICurrentUserProvider _currentUser = Substitute.For<ICurrentUserProvider>();
        private readonly Guid _productGuid = Guid.NewGuid();

        private UpdateProductCommandHandler CreateHandler() => new(_repository, _unitOfWork, _currentUser);

        private static Product CreateSampleProduct() => Product.Create(
            "20260810-1-1-1", 1, "Zapato deportivo", ProductServiceLineId.New(),
            null, null, null, null, null, null, true, true, true).Value;

        private static UpdateProductCommand CreateCommand(Guid id, string? code = null) => new(
            id, code, "Zapato deportivo v2", null, null, null, null,
            null, null, null, null, true, true, true,
            null, null, null, null, null, null,
            new List<Guid>(), new List<Guid>(), null, null, null);

        [Fact]
        public async Task Handle_WhenNotFoundOrNoPslAccess_ReturnsNotFound()
        {
            _repository.GetByIdAsync(Arg.Any<ProductId>(), Arg.Any<IReadOnlyList<Guid>>(), Arg.Any<CancellationToken>())
                .Returns((Product?)null);

            var result = await CreateHandler().Handle(CreateCommand(_productGuid), CancellationToken.None);

            Assert.True(result.IsError);
            Assert.Equal(ProductErrors.NotFound.Code, result.FirstError.Code);
        }

        [Fact]
        public async Task Handle_WhenNewCodeAlreadyUsedByAnotherProduct_ReturnsDuplicateCode()
        {
            var product = CreateSampleProduct();
            _repository.GetByIdAsync(Arg.Any<ProductId>(), Arg.Any<IReadOnlyList<Guid>>(), Arg.Any<CancellationToken>()).Returns(product);
            _repository.ExistsByCodeAsync("codigo-nuevo", excludeId: product.Id, Arg.Any<CancellationToken>()).Returns(true);

            var result = await CreateHandler().Handle(CreateCommand(_productGuid, "codigo-nuevo"), CancellationToken.None);

            Assert.True(result.IsError);
            Assert.Equal(ProductErrors.DuplicateCode.Code, result.FirstError.Code);
        }

        [Fact]
        public async Task Handle_WhenCodeUnchanged_SkipsDuplicateCheck()
        {
            var product = CreateSampleProduct();
            _repository.GetByIdAsync(Arg.Any<ProductId>(), Arg.Any<IReadOnlyList<Guid>>(), Arg.Any<CancellationToken>()).Returns(product);

            // Mismo código que ya tenía el producto — no debería ni consultar ExistsByCodeAsync.
            var result = await CreateHandler().Handle(CreateCommand(_productGuid, product.Code), CancellationToken.None);

            Assert.False(result.IsError);
            await _repository.DidNotReceive().ExistsByCodeAsync(Arg.Any<string>(), Arg.Any<ProductId?>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_WhenValid_UpdatesDetailsAndTaxesThenPersists()
        {
            var product = CreateSampleProduct();
            _repository.GetByIdAsync(Arg.Any<ProductId>(), Arg.Any<IReadOnlyList<Guid>>(), Arg.Any<CancellationToken>()).Returns(product);

            var result = await CreateHandler().Handle(CreateCommand(_productGuid), CancellationToken.None);

            Assert.False(result.IsError);
            Assert.Equal("Zapato deportivo v2", product.Name);
            await _repository.Received(1).UpdateAsync(product, Arg.Any<CancellationToken>());
            await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }
    }
}
