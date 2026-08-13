using Inventory.Application.Interfaces;
using Inventory.Application.ProductTypes.Commands.UpdateProductType;
using Inventory.Domain.DomainErrors;
using Inventory.Domain.Entities;
using Inventory.Domain.Repositories;
using Inventory.Domain.VO;
using NSubstitute;
using Xunit;

namespace Application.Tests
{
    public sealed class UpdateProductTypeCommandHandlerTests
    {
        private readonly IProductTypeRepository _repository = Substitute.For<IProductTypeRepository>();
        private readonly IInventoryUnitOfWork _unitOfWork = Substitute.For<IInventoryUnitOfWork>();
        private readonly Guid _typeGuid = Guid.NewGuid();

        private UpdateProductTypeCommandHandler CreateHandler() => new(_repository, _unitOfWork);

        [Fact]
        public async Task Handle_WhenNotFound_ReturnsNotFound()
        {
            _repository.GetByIdAsync(Arg.Any<ProductTypeId>(), Arg.Any<CancellationToken>()).Returns((ProductType?)null);

            var result = await CreateHandler().Handle(new UpdateProductTypeCommand(_typeGuid, "Bien", null), CancellationToken.None);

            Assert.True(result.IsError);
            Assert.Equal(ProductTypeErrors.NotFound.Code, result.FirstError.Code);
        }

        [Fact]
        public async Task Handle_WhenNameAlreadyUsedByAnother_ReturnsDuplicateName()
        {
            var productType = ProductType.CreateExisting(_typeGuid, "Bien", null, isActive: true);
            _repository.GetByIdAsync(Arg.Any<ProductTypeId>(), Arg.Any<CancellationToken>()).Returns(productType);
            _repository.ExistsByNameAsync("Servicio", excludeId: Arg.Any<ProductTypeId?>(), cancellationToken: Arg.Any<CancellationToken>())
                .Returns(true);

            var result = await CreateHandler().Handle(new UpdateProductTypeCommand(_typeGuid, "Servicio", null), CancellationToken.None);

            Assert.True(result.IsError);
            Assert.Equal(ProductTypeErrors.DuplicateName.Code, result.FirstError.Code);
        }

        [Fact]
        public async Task Handle_WhenValid_UpdatesAndReturnsResponse()
        {
            var productType = ProductType.CreateExisting(_typeGuid, "Bien", null, isActive: true);
            _repository.GetByIdAsync(Arg.Any<ProductTypeId>(), Arg.Any<CancellationToken>()).Returns(productType);
            _repository.ExistsByNameAsync(Arg.Any<string>(), excludeId: Arg.Any<ProductTypeId?>(), cancellationToken: Arg.Any<CancellationToken>())
                .Returns(false);

            var result = await CreateHandler().Handle(new UpdateProductTypeCommand(_typeGuid, "Servicio", "Producto intangible"), CancellationToken.None);

            Assert.False(result.IsError);
            Assert.Equal("Servicio", result.Value.Name);
            await _repository.Received(1).UpdateAsync(productType, Arg.Any<CancellationToken>());
            await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }
    }
}
