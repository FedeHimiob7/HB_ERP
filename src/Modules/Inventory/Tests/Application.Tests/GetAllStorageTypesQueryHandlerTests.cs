using Inventory.Application.StorageTypes.Queries.GetAll;
using Inventory.Domain.Entities;
using Inventory.Domain.Repositories;
using NSubstitute;
using Xunit;

namespace Application.Tests
{
    public sealed class GetAllStorageTypesQueryHandlerTests
    {
        private readonly IStorageTypeRepository _repository = Substitute.For<IStorageTypeRepository>();

        private GetAllStorageTypesQueryHandler CreateHandler() => new(_repository);

        [Fact]
        public async Task Handle_MapsAllItemsToResponse()
        {
            var items = new List<StorageType> { StorageType.CreateExisting(Guid.NewGuid(), "Refrigerado", null, isActive: true) };
            _repository.GetAllAsync(Arg.Any<CancellationToken>()).Returns(items);

            var result = await CreateHandler().Handle(new GetAllStorageTypesQuery(), CancellationToken.None);

            Assert.False(result.IsError);
            Assert.Single(result.Value);
            Assert.Equal("Refrigerado", result.Value[0].Name);
        }
    }
}
