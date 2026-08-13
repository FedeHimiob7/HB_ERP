using HB_ERP.SharedKernel.Domain.Primitives;
using Inventory.Application.Warehouses.Commands.DeactivateWarehouse;
using Inventory.Domain.DomainErrors;
using Inventory.Domain.Entities;
using Inventory.Domain.Repositories;
using Inventory.Domain.VO;
using NSubstitute;
using Xunit;

namespace Application.Tests
{
    public sealed class DeactivateWarehouseCommandHandlerTests
    {
        private readonly IWarehouseRepository _repository = Substitute.For<IWarehouseRepository>();
        private readonly Inventory.Application.Interfaces.IInventoryUnitOfWork _unitOfWork = Substitute.For<Inventory.Application.Interfaces.IInventoryUnitOfWork>();
        private readonly ICurrentUserProvider _currentUser = Substitute.For<ICurrentUserProvider>();
        private readonly Guid _warehouseGuid = Guid.NewGuid();

        private DeactivateWarehouseCommandHandler CreateHandler() => new(_repository, _unitOfWork, _currentUser);

        [Fact]
        public async Task Handle_WhenNotFound_ReturnsNotFound()
        {
            _repository.GetByIdAsync(Arg.Any<WarehouseId>(), Arg.Any<IReadOnlyList<Guid>>(), Arg.Any<CancellationToken>())
                .Returns((Warehouse?)null);

            var result = await CreateHandler().Handle(new DeactivateWarehouseCommand(_warehouseGuid), CancellationToken.None);

            Assert.True(result.IsError);
            Assert.Equal(WarehouseErrors.NotFound.Code, result.FirstError.Code);
        }

        [Fact]
        public async Task Handle_WhenValid_DeactivatesAndPersists()
        {
            var warehouse = Warehouse.CreateExisting(_warehouseGuid, Guid.NewGuid(), "Deposito Central", null, null, null, isActive: true);
            _repository.GetByIdAsync(Arg.Any<WarehouseId>(), Arg.Any<IReadOnlyList<Guid>>(), Arg.Any<CancellationToken>()).Returns(warehouse);

            var result = await CreateHandler().Handle(new DeactivateWarehouseCommand(_warehouseGuid), CancellationToken.None);

            Assert.False(result.IsError);
            Assert.False(warehouse.IsActive);
            await _repository.Received(1).UpdateAsync(warehouse, Arg.Any<CancellationToken>());
            await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }
    }
}
