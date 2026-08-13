using HB_ERP.SharedKernel.Domain.Primitives;
using Inventory.Application.Warehouses.Commands.UpdateWarehouse;
using Inventory.Domain.DomainErrors;
using Inventory.Domain.Entities;
using Inventory.Domain.Repositories;
using Inventory.Domain.VO;
using MasterData.Domain.Entities;
using MasterData.Domain.Repositories;
using MasterData.Domain.VO;
using NSubstitute;
using Xunit;

namespace Application.Tests
{
    public sealed class UpdateWarehouseCommandHandlerTests
    {
        private readonly IWarehouseRepository _repository = Substitute.For<IWarehouseRepository>();
        private readonly IProductServiceLineRepository _pslRepository = Substitute.For<IProductServiceLineRepository>();
        private readonly Inventory.Application.Interfaces.IInventoryUnitOfWork _unitOfWork = Substitute.For<Inventory.Application.Interfaces.IInventoryUnitOfWork>();
        private readonly ICurrentUserProvider _currentUser = Substitute.For<ICurrentUserProvider>();
        private readonly Guid _warehouseGuid = Guid.NewGuid();
        private readonly Guid _pslGuid = Guid.NewGuid();

        private UpdateWarehouseCommandHandler CreateHandler() => new(_repository, _pslRepository, _unitOfWork, _currentUser);

        [Fact]
        public async Task Handle_WhenWarehouseDoesNotExistOrNoPslAccess_ReturnsNotFound()
        {
            _repository.GetByIdAsync(Arg.Any<WarehouseId>(), Arg.Any<IReadOnlyList<Guid>>(), Arg.Any<CancellationToken>())
                .Returns((Warehouse?)null);

            var command = new UpdateWarehouseCommand(_warehouseGuid, _pslGuid, "Deposito Central", null, null, null);
            var result = await CreateHandler().Handle(command, CancellationToken.None);

            Assert.True(result.IsError);
            Assert.Equal(WarehouseErrors.NotFound.Code, result.FirstError.Code);
        }

        [Fact]
        public async Task Handle_WhenValid_UpdatesAndReturnsResponse()
        {
            var warehouse = Warehouse.CreateExisting(_warehouseGuid, _pslGuid, "Deposito Viejo", null, null, null, isActive: true);
            _repository.GetByIdAsync(Arg.Any<WarehouseId>(), Arg.Any<IReadOnlyList<Guid>>(), Arg.Any<CancellationToken>()).Returns(warehouse);
            _pslRepository.GetByIdAsync(Arg.Any<ProductServiceLineId>(), Arg.Any<CancellationToken>())
                .Returns(ProductServiceLine.CreateExisting(_pslGuid, "desc", "Calzado", isActive: true));
            _repository.ExistsByNameInPslAsync(Arg.Any<string>(), Arg.Any<ProductServiceLineId>(), excludeId: Arg.Any<WarehouseId?>(), cancellationToken: Arg.Any<CancellationToken>())
                .Returns(false);

            var command = new UpdateWarehouseCommand(_warehouseGuid, _pslGuid, "Deposito Central", null, null, null);
            var result = await CreateHandler().Handle(command, CancellationToken.None);

            Assert.False(result.IsError);
            Assert.Equal("Deposito Central", result.Value.Name);
            await _repository.Received(1).UpdateAsync(warehouse, Arg.Any<CancellationToken>());
            await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }
    }
}
