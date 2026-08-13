using Inventory.Domain.Entities;
using Inventory.Domain.VO;
using Xunit;

namespace Domain.Tests
{
    public sealed class ProductSubCategoryTests
    {
        private static readonly ProductCategoryId ValidCategoryId = ProductCategoryId.New();

        [Fact]
        public void Create_WithValidData_Succeeds()
        {
            var result = ProductSubCategory.Create(ValidCategoryId, "Zapatillas", null);

            Assert.False(result.IsError);
            Assert.Equal("Zapatillas", result.Value.Name);
            Assert.True(result.Value.IsActive);
        }

        [Fact]
        public void Create_WithEmptyCategoryId_ReturnsInvalidCategory()
        {
            var result = ProductSubCategory.Create(ProductCategoryId.Create(Guid.Empty), "Zapatillas", null);

            Assert.True(result.IsError);
            Assert.Equal("ProductSubCategory.InvalidCategory", result.FirstError.Code);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Create_WithBlankName_Fails(string name)
        {
            var result = ProductSubCategory.Create(ValidCategoryId, name, null);

            Assert.True(result.IsError);
            Assert.Equal("ProductSubCategory.NameIsRequired", result.FirstError.Code);
        }

        [Fact]
        public void UpdateDetails_WithValidData_UpdatesAllFields()
        {
            var subCategory = ProductSubCategory.Create(ValidCategoryId, "Zapatillas", null).Value;
            var newCategoryId = ProductCategoryId.New();

            var result = subCategory.UpdateDetails(newCategoryId, "Sandalias", "Calzado abierto");

            Assert.False(result.IsError);
            Assert.Equal(newCategoryId, subCategory.ProductCategoryId);
            Assert.Equal("Sandalias", subCategory.Name);
        }

        [Fact]
        public void Deactivate_ThenActivate_TogglesIsActive()
        {
            var subCategory = ProductSubCategory.Create(ValidCategoryId, "Zapatillas", null).Value;

            subCategory.Deactivate();
            Assert.False(subCategory.IsActive);

            subCategory.Activate();
            Assert.True(subCategory.IsActive);
        }
    }
}
