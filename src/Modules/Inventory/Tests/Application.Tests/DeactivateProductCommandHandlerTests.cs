using HB_ERP.SharedKernel.Domain.Primitives;
using Inventory.Application.Products.Commands.DeactivateProduct;
using Inventory.Domain.DomainErrors;
using Inventory.Domain.Entities;
using Inventory.Domain.Repositories;
using Inventory.Domain.VO;
using MasterData.Domain.VO;
using NSubstitute;
using Xunit;

namespace Application.Tests
{
    public sealed class DeactivateProductCommandHandlerTests
    {
        private readonly IProductRepository _repository = Substitute.For<IProductRepository>();
        private readonly Inventory.Application.Interfaces.IInventoryUnitOfWork _unitOfWork = Substitute.For<Inventory.Application.Interfaces.IInventoryUnitOfWork>();
        private readonly ICurrentUserProvider _currentUser = Substitute.For<ICurrentUserProvider>();
        private readonly Guid _productGuid = Guid.NewGuid();

        private DeactivateProductCommandHandler CreateHandler() => new(_repository, _unitOfWork, _currentUser);

        [Fact]
        public async Task Handle_WhenNotFoundOrNoPslAccess_ReturnsNotFound()
        {
            _repository.GetByIdAsync(Arg.Any<ProductId>(), Arg.Any<IReadOnlyList<Guid>>(), Arg.Any<CancellationToken>())
                .Returns((Product?)null);

            var result = await CreateHandler().Handle(new DeactivateProductCommand(_productGuid), CancellationToken.None);

            Assert.True(result.IsError);
            Assert.Equal(ProductErrors.NotFound.Code, result.FirstError.Code);
        }

        [Fact]
        public async Task Handle_WhenValid_DeactivatesAndPersists()
        {
            var product = Product.Create(
                "20260810-1-1-1", 1, "Zapato deportivo", ProductServiceLineId.New(),
                null, null, null, null, null, null, true, true, true).Value;
            _repository.GetByIdAsync(Arg.Any<ProductId>(), Arg.Any<IReadOnlyList<Guid>>(), Arg.Any<CancellationToken>()).Returns(product);

            var result = await CreateHandler().Handle(new DeactivateProductCommand(_productGuid), CancellationToken.None);

            Assert.False(result.IsError);
            Assert.False(product.IsActive);
            await _repository.Received(1).UpdateAsync(product, Arg.Any<CancellationToken>());
            await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }
    }
}
