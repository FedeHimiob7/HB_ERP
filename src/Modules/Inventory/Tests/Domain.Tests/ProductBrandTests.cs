using Inventory.Domain.Entities;
using Xunit;

namespace Domain.Tests
{
    public sealed class ProductBrandTests
    {
        [Fact]
        public void Create_WithValidData_Succeeds()
        {
            var result = ProductBrand.Create("Nike", "Marca deportiva");

            Assert.False(result.IsError);
            Assert.Equal("Nike", result.Value.Name);
            Assert.True(result.Value.IsActive);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Create_WithBlankName_Fails(string name)
        {
            var result = ProductBrand.Create(name, null);

            Assert.True(result.IsError);
            Assert.Equal("ProductBrand.NameIsRequired", result.FirstError.Code);
        }

        [Fact]
        public void UpdateDetails_WithValidData_UpdatesAllFields()
        {
            var brand = ProductBrand.Create("Nike", null).Value;

            var result = brand.UpdateDetails("Adidas", "Marca deportiva");

            Assert.False(result.IsError);
            Assert.Equal("Adidas", brand.Name);
            Assert.Equal("Marca deportiva", brand.Description);
        }

        [Fact]
        public void Deactivate_ThenActivate_TogglesIsActive()
        {
            var brand = ProductBrand.Create("Nike", null).Value;

            brand.Deactivate();
            Assert.False(brand.IsActive);

            brand.Activate();
            Assert.True(brand.IsActive);
        }
    }
}
