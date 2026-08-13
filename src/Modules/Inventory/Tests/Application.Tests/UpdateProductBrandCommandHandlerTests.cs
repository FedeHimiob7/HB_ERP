using Inventory.Application.Interfaces;
using Inventory.Application.ProductBrands.Commands.UpdateProductBrand;
using Inventory.Domain.DomainErrors;
using Inventory.Domain.Entities;
using Inventory.Domain.Repositories;
using Inventory.Domain.VO;
using NSubstitute;
using Xunit;

namespace Application.Tests
{
    public sealed class UpdateProductBrandCommandHandlerTests
    {
        private readonly IProductBrandRepository _repository = Substitute.For<IProductBrandRepository>();
        private readonly IInventoryUnitOfWork _unitOfWork = Substitute.For<IInventoryUnitOfWork>();
        private readonly Guid _brandGuid = Guid.NewGuid();

        private UpdateProductBrandCommandHandler CreateHandler() => new(_repository, _unitOfWork);

        [Fact]
        public async Task Handle_WhenNotFound_ReturnsNotFound()
        {
            _repository.GetByIdAsync(Arg.Any<ProductBrandId>(), Arg.Any<CancellationToken>()).Returns((ProductBrand?)null);

            var result = await CreateHandler().Handle(new UpdateProductBrandCommand(_brandGuid, "Nike", null), CancellationToken.None);

            Assert.True(result.IsError);
            Assert.Equal(ProductBrandErrors.NotFound.Code, result.FirstError.Code);
        }

        [Fact]
        public async Task Handle_WhenValid_UpdatesAndReturnsResponse()
        {
            var brand = ProductBrand.CreateExisting(_brandGuid, "Nike", null, isActive: true);
            _repository.GetByIdAsync(Arg.Any<ProductBrandId>(), Arg.Any<CancellationToken>()).Returns(brand);
            _repository.ExistsByNameAsync(Arg.Any<string>(), excludeId: Arg.Any<ProductBrandId?>(), cancellationToken: Arg.Any<CancellationToken>())
                .Returns(false);

            var result = await CreateHandler().Handle(new UpdateProductBrandCommand(_brandGuid, "Adidas", "Marca deportiva"), CancellationToken.None);

            Assert.False(result.IsError);
            Assert.Equal("Adidas", result.Value.Name);
            await _repository.Received(1).UpdateAsync(brand, Arg.Any<CancellationToken>());
            await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }
    }
}
