using Inventory.Domain.Entities;
using MasterData.Domain.VO;
using Xunit;

namespace Domain.Tests
{
    public sealed class WarehouseTests
    {
        private static readonly ProductServiceLineId ValidPslId = ProductServiceLineId.New();

        [Fact]
        public void Create_WithValidData_Succeeds()
        {
            var result = Warehouse.Create(ValidPslId, "Deposito Central", "Deposito principal", "10.5", "-66.9");

            Assert.False(result.IsError);
            Assert.Equal("Deposito Central", result.Value.Name);
            Assert.Equal("10.5", result.Value.Latitude);
            Assert.True(result.Value.IsActive);
        }

        [Fact]
        public void Create_WithEmptyPslId_ReturnsInvalidProductServiceLine()
        {
            var result = Warehouse.Create(ProductServiceLineId.Create(Guid.Empty), "Deposito Central", null, null, null);

            Assert.True(result.IsError);
            Assert.Equal("Warehouse.InvalidProductServiceLine", result.FirstError.Code);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Create_WithBlankName_Fails(string name)
        {
            var result = Warehouse.Create(ValidPslId, name, null, null, null);

            Assert.True(result.IsError);
            Assert.Equal("Warehouse.NameIsRequired", result.FirstError.Code);
        }

        [Fact]
        public void UpdateDetails_WithValidData_UpdatesAllFields()
        {
            var warehouse = Warehouse.Create(ValidPslId, "Deposito Central", null, null, null).Value;
            var newPslId = ProductServiceLineId.New();

            var result = warehouse.UpdateDetails(newPslId, "Deposito Norte", "Deposito secundario", "11.0", "-67.0");

            Assert.False(result.IsError);
            Assert.Equal(newPslId, warehouse.ProductServiceLineId);
            Assert.Equal("Deposito Norte", warehouse.Name);
            Assert.Equal("11.0", warehouse.Latitude);
        }

        [Fact]
        public void Deactivate_ThenActivate_TogglesIsActive()
        {
            var warehouse = Warehouse.Create(ValidPslId, "Deposito Central", null, null, null).Value;

            warehouse.Deactivate();
            Assert.False(warehouse.IsActive);

            warehouse.Activate();
            Assert.True(warehouse.IsActive);
        }
    }
}
