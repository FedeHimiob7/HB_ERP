using Inventory.Domain.Entities;
using Xunit;

namespace Domain.Tests
{
    public sealed class ProductTypeTests
    {
        [Fact]
        public void Create_WithValidData_Succeeds()
        {
            var result = ProductType.Create("Bien", "Producto tangible");

            Assert.False(result.IsError);
            Assert.Equal("Bien", result.Value.Name);
            Assert.True(result.Value.IsActive);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Create_WithBlankName_Fails(string name)
        {
            var result = ProductType.Create(name, null);

            Assert.True(result.IsError);
            Assert.Equal("ProductType.NameIsRequired", result.FirstError.Code);
        }

        [Fact]
        public void UpdateDetails_WithValidData_UpdatesAllFields()
        {
            var productType = ProductType.Create("Bien", null).Value;

            var result = productType.UpdateDetails("Servicio", "Producto intangible");

            Assert.False(result.IsError);
            Assert.Equal("Servicio", productType.Name);
            Assert.Equal("Producto intangible", productType.Description);
        }

        [Fact]
        public void Deactivate_ThenActivate_TogglesIsActive()
        {
            var productType = ProductType.Create("Bien", null).Value;

            productType.Deactivate();
            Assert.False(productType.IsActive);

            productType.Activate();
            Assert.True(productType.IsActive);
        }
    }
}
