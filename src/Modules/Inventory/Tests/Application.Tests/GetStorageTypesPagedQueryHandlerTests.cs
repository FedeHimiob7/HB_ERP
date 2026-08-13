using Inventory.Application.StorageTypes.Queries.GetPaged;
using Inventory.Domain.Entities;
using Inventory.Domain.Repositories;
using Inventory.Domain.SearchParametersModel;
using NSubstitute;
using Xunit;

namespace Application.Tests
{
    public sealed class GetStorageTypesPagedQueryHandlerTests
    {
        private readonly IStorageTypeRepository _repository = Substitute.For<IStorageTypeRepository>();

        private GetStorageTypesPagedQueryHandler CreateHandler() => new(_repository);

        [Fact]
        public async Task Handle_ReturnsPagedResultUsingFilterPassedByCaller()
        {
            var items = new List<StorageType> { StorageType.CreateExisting(Guid.NewGuid(), "Refrigerado", null, isActive: true) };
            var filter = new StorageTypeFilter(1, 10, "Refri");
            _repository.GetPagedAsync(filter, Arg.Any<CancellationToken>()).Returns((items, 3));

            var result = await CreateHandler().Handle(new GetStorageTypesPagedQuery(filter), CancellationToken.None);

            Assert.False(result.IsError);
            Assert.Single(result.Value.Items);
            Assert.Equal(3, result.Value.TotalCount);
        }
    }
}
