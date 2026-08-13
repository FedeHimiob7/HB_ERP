using Inventory.Application.Interfaces;
using Inventory.Application.StorageTypes.Commands.DeactivateStorageType;
using Inventory.Domain.DomainErrors;
using Inventory.Domain.Entities;
using Inventory.Domain.Repositories;
using Inventory.Domain.VO;
using NSubstitute;
using Xunit;

namespace Application.Tests
{
    public sealed class DeactivateStorageTypeCommandHandlerTests
    {
        private readonly IStorageTypeRepository _repository = Substitute.For<IStorageTypeRepository>();
        private readonly IInventoryUnitOfWork _unitOfWork = Substitute.For<IInventoryUnitOfWork>();
        private readonly Guid _typeGuid = Guid.NewGuid();

        private DeactivateStorageTypeCommandHandler CreateHandler() => new(_repository, _unitOfWork);

        [Fact]
        public async Task Handle_WhenNotFound_ReturnsNotFound()
        {
            _repository.GetByIdAsync(Arg.Any<StorageTypeId>(), Arg.Any<CancellationToken>()).Returns((StorageType?)null);

            var result = await CreateHandler().Handle(new DeactivateStorageTypeCommand(_typeGuid), CancellationToken.None);

            Assert.True(result.IsError);
            Assert.Equal(StorageTypeErrors.NotFound.Code, result.FirstError.Code);
        }

        [Fact]
        public async Task Handle_WhenValid_DeactivatesAndPersists()
        {
            var storageType = StorageType.CreateExisting(_typeGuid, "Refrigerado", null, isActive: true);
            _repository.GetByIdAsync(Arg.Any<StorageTypeId>(), Arg.Any<CancellationToken>()).Returns(storageType);

            var result = await CreateHandler().Handle(new DeactivateStorageTypeCommand(_typeGuid), CancellationToken.None);

            Assert.False(result.IsError);
            Assert.False(storageType.IsActive);
            await _repository.Received(1).UpdateAsync(storageType, Arg.Any<CancellationToken>());
            await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }
    }
}
