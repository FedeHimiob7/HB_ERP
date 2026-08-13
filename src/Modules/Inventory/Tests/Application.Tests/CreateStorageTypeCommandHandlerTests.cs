using Inventory.Application.Interfaces;
using Inventory.Application.StorageTypes.Commands.CreateStorageType;
using Inventory.Domain.DomainErrors;
using Inventory.Domain.Entities;
using Inventory.Domain.Repositories;
using NSubstitute;
using Xunit;

namespace Application.Tests
{
    public sealed class CreateStorageTypeCommandHandlerTests
    {
        private readonly IStorageTypeRepository _repository = Substitute.For<IStorageTypeRepository>();
        private readonly IInventoryUnitOfWork _unitOfWork = Substitute.For<IInventoryUnitOfWork>();

        private CreateStorageTypeCommandHandler CreateHandler() => new(_repository, _unitOfWork);

        [Fact]
        public async Task Handle_WhenNameAlreadyExists_ReturnsDuplicateName()
        {
            _repository.ExistsByNameAsync("Refrigerado", cancellationToken: Arg.Any<CancellationToken>()).Returns(true);

            var result = await CreateHandler().Handle(new CreateStorageTypeCommand("Refrigerado", null), CancellationToken.None);

            Assert.True(result.IsError);
            Assert.Equal(StorageTypeErrors.DuplicateName.Code, result.FirstError.Code);
        }

        [Fact]
        public async Task Handle_WhenValid_PersistsAndReturnsId()
        {
            _repository.ExistsByNameAsync(Arg.Any<string>(), cancellationToken: Arg.Any<CancellationToken>()).Returns(false);

            var result = await CreateHandler().Handle(new CreateStorageTypeCommand("Refrigerado", "Requiere cadena de frio"), CancellationToken.None);

            Assert.False(result.IsError);
            await _repository.Received(1).AddAsync(Arg.Any<StorageType>(), Arg.Any<CancellationToken>());
            await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }
    }
}
