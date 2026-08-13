using MasterData.Domain.Entities;
using Xunit;

namespace Domain.Tests
{
    public sealed class ProductServiceLineTests
    {
        [Fact]
        public void Create_WithValidData_Succeeds()
        {
            var result = ProductServiceLine.Create("Linea de calzado", "Calzado");

            Assert.False(result.IsError);
            Assert.Equal("Calzado", result.Value.Name);
            Assert.Equal("Linea de calzado", result.Value.Description);
            Assert.True(result.Value.IsActive);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Create_WithBlankName_Fails(string name)
        {
            var result = ProductServiceLine.Create("Linea de calzado", name);

            Assert.True(result.IsError);
            Assert.Equal("ProductServiceLine.NameIsRequired", result.FirstError.Code);
        }

        [Fact]
        public void UpdateDetails_WithValidData_UpdatesAllFields()
        {
            var psl = ProductServiceLine.Create("Linea de calzado", "Calzado").Value;

            var result = psl.UpdateDetails("Linea de ropa", "Ropa");

            Assert.False(result.IsError);
            Assert.Equal("Ropa", psl.Name);
            Assert.Equal("Linea de ropa", psl.Description);
        }

        [Fact]
        public void UpdateDetails_WithBlankName_Fails()
        {
            var psl = ProductServiceLine.Create("Linea de calzado", "Calzado").Value;

            var result = psl.UpdateDetails("Linea de ropa", "");

            Assert.True(result.IsError);
            Assert.Equal("Calzado", psl.Name);
        }

        [Fact]
        public void Deactivate_ThenActivate_TogglesIsActive()
        {
            var psl = ProductServiceLine.Create("Linea de calzado", "Calzado").Value;

            psl.Deactivate();
            Assert.False(psl.IsActive);

            psl.Activate();
            Assert.True(psl.IsActive);
        }
    }
}
