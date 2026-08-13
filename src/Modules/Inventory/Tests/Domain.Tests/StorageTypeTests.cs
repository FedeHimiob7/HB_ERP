using Inventory.Domain.Entities;
using Xunit;

namespace Domain.Tests
{
    public sealed class StorageTypeTests
    {
        [Fact]
        public void Create_WithValidData_Succeeds()
        {
            var result = StorageType.Create("Refrigerado", "Requiere cadena de frio");

            Assert.False(result.IsError);
            Assert.Equal("Refrigerado", result.Value.Name);
            Assert.True(result.Value.IsActive);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Create_WithBlankName_Fails(string name)
        {
            var result = StorageType.Create(name, null);

            Assert.True(result.IsError);
            Assert.Equal("StorageType.NameIsRequired", result.FirstError.Code);
        }

        [Fact]
        public void UpdateDetails_WithValidData_UpdatesAllFields()
        {
            var storageType = StorageType.Create("Estanteria", null).Value;

            var result = storageType.UpdateDetails("Refrigerado", "Requiere cadena de frio");

            Assert.False(result.IsError);
            Assert.Equal("Refrigerado", storageType.Name);
            Assert.Equal("Requiere cadena de frio", storageType.Description);
        }

        [Fact]
        public void Deactivate_ThenActivate_TogglesIsActive()
        {
            var storageType = StorageType.Create("Estanteria", null).Value;

            storageType.Deactivate();
            Assert.False(storageType.IsActive);

            storageType.Activate();
            Assert.True(storageType.IsActive);
        }
    }
}
