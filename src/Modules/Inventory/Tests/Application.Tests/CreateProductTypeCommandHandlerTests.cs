using Inventory.Application.Interfaces;
using Inventory.Application.ProductTypes.Commands.CreateProductType;
using Inventory.Domain.DomainErrors;
using Inventory.Domain.Entities;
using Inventory.Domain.Repositories;
using NSubstitute;
using Xunit;

namespace Application.Tests
{
    public sealed class CreateProductTypeCommandHandlerTests
    {
        private readonly IProductTypeRepository _repository = Substitute.For<IProductTypeRepository>();
        private readonly IInventoryUnitOfWork _unitOfWork = Substitute.For<IInventoryUnitOfWork>();

        private CreateProductTypeCommandHandler CreateHandler() => new(_repository, _unitOfWork);

        [Fact]
        public async Task Handle_WhenNameAlreadyExists_ReturnsDuplicateName()
        {
            _repository.ExistsByNameAsync("Bien", cancellationToken: Arg.Any<CancellationToken>()).Returns(true);

            var result = await CreateHandler().Handle(new CreateProductTypeCommand("Bien", null), CancellationToken.None);

            Assert.True(result.IsError);
            Assert.Equal(ProductTypeErrors.DuplicateName.Code, result.FirstError.Code);
        }

        [Fact]
        public async Task Handle_WhenValid_PersistsAndReturnsId()
        {
            _repository.ExistsByNameAsync(Arg.Any<string>(), cancellationToken: Arg.Any<CancellationToken>()).Returns(false);

            var result = await CreateHandler().Handle(new CreateProductTypeCommand("Bien", "Producto tangible"), CancellationToken.None);

            Assert.False(result.IsError);
            Assert.NotEqual(Guid.Empty, result.Value);
            await _repository.Received(1).AddAsync(Arg.Any<ProductType>(), Arg.Any<CancellationToken>());
            await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }
    }
}
