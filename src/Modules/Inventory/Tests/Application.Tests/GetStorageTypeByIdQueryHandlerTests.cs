using Inventory.Application.StorageTypes.Queries.GetById;
using Inventory.Domain.DomainErrors;
using Inventory.Domain.Entities;
using Inventory.Domain.Repositories;
using Inventory.Domain.VO;
using NSubstitute;
using Xunit;

namespace Application.Tests
{
    public sealed class GetStorageTypeByIdQueryHandlerTests
    {
        private readonly IStorageTypeRepository _repository = Substitute.For<IStorageTypeRepository>();
        private readonly Guid _typeGuid = Guid.NewGuid();

        private GetStorageTypeByIdQueryHandler CreateHandler() => new(_repository);

        [Fact]
        public async Task Handle_WhenNotFound_ReturnsNotFound()
        {
            _repository.GetByIdAsync(Arg.Any<StorageTypeId>(), Arg.Any<CancellationToken>()).Returns((StorageType?)null);

            var result = await CreateHandler().Handle(new GetStorageTypeByIdQuery(_typeGuid), CancellationToken.None);

            Assert.True(result.IsError);
            Assert.Equal(StorageTypeErrors.NotFound.Code, result.FirstError.Code);
        }

        [Fact]
        public async Task Handle_WhenFound_ReturnsResponse()
        {
            var storageType = StorageType.CreateExisting(_typeGuid, "Refrigerado", null, isActive: true);
            _repository.GetByIdAsync(Arg.Any<StorageTypeId>(), Arg.Any<CancellationToken>()).Returns(storageType);

            var result = await CreateHandler().Handle(new GetStorageTypeByIdQuery(_typeGuid), CancellationToken.None);

            Assert.False(result.IsError);
            Assert.Equal("Refrigerado", result.Value.Name);
        }
    }
}
