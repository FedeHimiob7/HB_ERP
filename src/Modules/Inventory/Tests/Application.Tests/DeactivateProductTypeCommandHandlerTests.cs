using Inventory.Application.Interfaces;
using Inventory.Application.ProductTypes.Commands.DeactivateProductType;
using Inventory.Domain.DomainErrors;
using Inventory.Domain.Entities;
using Inventory.Domain.Repositories;
using Inventory.Domain.VO;
using NSubstitute;
using Xunit;

namespace Application.Tests
{
    public sealed class DeactivateProductTypeCommandHandlerTests
    {
        private readonly IProductTypeRepository _repository = Substitute.For<IProductTypeRepository>();
        private readonly IInventoryUnitOfWork _unitOfWork = Substitute.For<IInventoryUnitOfWork>();
        private readonly Guid _typeGuid = Guid.NewGuid();

        private DeactivateProductTypeCommandHandler CreateHandler() => new(_repository, _unitOfWork);

        [Fact]
        public async Task Handle_WhenNotFound_ReturnsNotFound()
        {
            _repository.GetByIdAsync(Arg.Any<ProductTypeId>(), Arg.Any<CancellationToken>()).Returns((ProductType?)null);

            var result = await CreateHandler().Handle(new DeactivateProductTypeCommand(_typeGuid), CancellationToken.None);

            Assert.True(result.IsError);
            Assert.Equal(ProductTypeErrors.NotFound.Code, result.FirstError.Code);
        }

        [Fact]
        public async Task Handle_WhenValid_DeactivatesAndPersists()
        {
            var productType = ProductType.CreateExisting(_typeGuid, "Bien", null, isActive: true);
            _repository.GetByIdAsync(Arg.Any<ProductTypeId>(), Arg.Any<CancellationToken>()).Returns(productType);

            var result = await CreateHandler().Handle(new DeactivateProductTypeCommand(_typeGuid), CancellationToken.None);

            Assert.False(result.IsError);
            Assert.False(productType.IsActive);
            await _repository.Received(1).UpdateAsync(productType, Arg.Any<CancellationToken>());
            await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }
    }
}
