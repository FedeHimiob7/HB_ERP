using Inventory.Domain.Entities;
using MasterData.Domain.VO;
using Xunit;

namespace Domain.Tests
{
    public sealed class ProductCategoryTests
    {
        private static readonly ProductServiceLineId ValidPslId = ProductServiceLineId.New();

        [Fact]
        public void Create_WithValidData_Succeeds()
        {
            var result = ProductCategory.Create(ValidPslId, null, "Calzado", null);

            Assert.False(result.IsError);
            Assert.Equal("Calzado", result.Value.Name);
            Assert.True(result.Value.IsActive);
        }

        [Fact]
        public void Create_WithEmptyPslId_ReturnsInvalidProductServiceLine()
        {
            var result = ProductCategory.Create(ProductServiceLineId.Create(Guid.Empty), null, "Calzado", null);

            Assert.True(result.IsError);
            Assert.Equal("ProductCategory.InvalidProductServiceLine", result.FirstError.Code);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Create_WithBlankName_Fails(string name)
        {
            var result = ProductCategory.Create(ValidPslId, null, name, null);

            Assert.True(result.IsError);
            Assert.Equal("ProductCategory.NameIsRequired", result.FirstError.Code);
        }

        [Fact]
        public void UpdateDetails_WithValidData_UpdatesAllFields()
        {
            var category = ProductCategory.Create(ValidPslId, null, "Calzado", null).Value;
            var newPslId = ProductServiceLineId.New();
            var typeId = new Inventory.Domain.VO.ProductTypeId(Guid.NewGuid());

            var result = category.UpdateDetails(newPslId, typeId, "Ropa", "Ropa deportiva");

            Assert.False(result.IsError);
            Assert.Equal(newPslId, category.ProductServiceLineId);
            Assert.Equal(typeId, category.ProductTypeId);
            Assert.Equal("Ropa", category.Name);
        }

        [Fact]
        public void Deactivate_ThenActivate_TogglesIsActive()
        {
            var category = ProductCategory.Create(ValidPslId, null, "Calzado", null).Value;

            category.Deactivate();
            Assert.False(category.IsActive);

            category.Activate();
            Assert.True(category.IsActive);
        }
    }
}
