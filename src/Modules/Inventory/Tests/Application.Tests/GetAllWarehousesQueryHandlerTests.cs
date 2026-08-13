using HB_ERP.SharedKernel.Domain.Primitives;
using Inventory.Application.Warehouses.Queries.GetAll;
using Inventory.Domain.Entities;
using Inventory.Domain.Repositories;
using MasterData.Domain.VO;
using NSubstitute;
using Xunit;

namespace Application.Tests
{
    public sealed class GetAllWarehousesQueryHandlerTests
    {
        private readonly IWarehouseRepository _repository = Substitute.For<IWarehouseRepository>();
        private readonly ICurrentUserProvider _currentUser = Substitute.For<ICurrentUserProvider>();

        private GetAllWarehousesQueryHandler CreateHandler() => new(_repository, _currentUser);

        [Fact]
        public async Task Handle_PassesCurrentUserAllowedPslIdsToRepository()
        {
            var allowedIds = new List<Guid> { Guid.NewGuid() };
            _currentUser.PslIds.Returns(allowedIds);
            _repository.GetAllAsync(Arg.Any<IReadOnlyList<Guid>>(), Arg.Any<ProductServiceLineId?>(), Arg.Any<CancellationToken>())
                .Returns(new List<Warehouse>());

            await CreateHandler().Handle(new GetAllWarehousesQuery(), CancellationToken.None);

            await _repository.Received(1).GetAllAsync(allowedIds, Arg.Any<ProductServiceLineId?>(), Arg.Any<CancellationToken>());
        }
    }
}
