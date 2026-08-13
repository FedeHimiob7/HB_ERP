using Inventory.Application.Interfaces;
using Inventory.Application.ProductBrands.Commands.CreateProductBrand;
using Inventory.Domain.DomainErrors;
using Inventory.Domain.Entities;
using Inventory.Domain.Repositories;
using NSubstitute;
using Xunit;

namespace Application.Tests
{
    public sealed class CreateProductBrandCommandHandlerTests
    {
        private readonly IProductBrandRepository _repository = Substitute.For<IProductBrandRepository>();
        private readonly IInventoryUnitOfWork _unitOfWork = Substitute.For<IInventoryUnitOfWork>();

        private CreateProductBrandCommandHandler CreateHandler() => new(_repository, _unitOfWork);

        [Fact]
        public async Task Handle_WhenNameAlreadyExists_ReturnsDuplicateName()
        {
            _repository.ExistsByNameAsync("Nike", cancellationToken: Arg.Any<CancellationToken>()).Returns(true);

            var result = await CreateHandler().Handle(new CreateProductBrandCommand("Nike", null), CancellationToken.None);

            Assert.True(result.IsError);
            Assert.Equal(ProductBrandErrors.DuplicateName.Code, result.FirstError.Code);
        }

        [Fact]
        public async Task Handle_WhenValid_PersistsAndReturnsId()
        {
            _repository.ExistsByNameAsync(Arg.Any<string>(), cancellationToken: Arg.Any<CancellationToken>()).Returns(false);

            var result = await CreateHandler().Handle(new CreateProductBrandCommand("Nike", "Marca deportiva"), CancellationToken.None);

            Assert.False(result.IsError);
            await _repository.Received(1).AddAsync(Arg.Any<ProductBrand>(), Arg.Any<CancellationToken>());
            await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }
    }
}
