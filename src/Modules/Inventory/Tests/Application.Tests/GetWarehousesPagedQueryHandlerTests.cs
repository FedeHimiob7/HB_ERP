using HB_ERP.SharedKernel.Domain.Primitives;
using Inventory.Application.Warehouses.Queries.GetPaged;
using Inventory.Domain.Entities;
using Inventory.Domain.Repositories;
using Inventory.Domain.SearchParametersModel;
using NSubstitute;
using Xunit;

namespace Application.Tests
{
    public sealed class GetWarehousesPagedQueryHandlerTests
    {
        private readonly IWarehouseRepository _repository = Substitute.For<IWarehouseRepository>();
        private readonly ICurrentUserProvider _currentUser = Substitute.For<ICurrentUserProvider>();

        private GetWarehousesPagedQueryHandler CreateHandler() => new(_repository, _currentUser);

        [Fact]
        public async Task Handle_PassesAllowedPslIdsAndReturnsPagedResult()
        {
            var allowedIds = new List<Guid> { Guid.NewGuid() };
            _currentUser.PslIds.Returns(allowedIds);
            var items = new List<Warehouse> { Warehouse.CreateExisting(Guid.NewGuid(), allowedIds[0], "Deposito Central", null, null, null, isActive: true) };
            var filter = new WarehouseFilter(1, 10, "Central");
            _repository.GetPagedAsync(filter, allowedIds, Arg.Any<CancellationToken>()).Returns((items, 1));

            var result = await CreateHandler().Handle(new GetWarehousesPagedQuery(filter), CancellationToken.None);

            Assert.False(result.IsError);
            Assert.Single(result.Value.Items);
            Assert.Equal(1, result.Value.TotalCount);
        }
    }
}
