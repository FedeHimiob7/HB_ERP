using MasterData.Domain.Entities;
using MasterData.Domain.Enums;
using Xunit;

namespace Domain.Tests
{
    public sealed class TaxTests
    {
        [Fact]
        public void Create_WithValidData_Succeeds()
        {
            var result = Tax.Create("IVA General", TaxType.IVA);

            Assert.False(result.IsError);
            Assert.Equal("IVA General", result.Value.Name);
            Assert.Equal(TaxType.IVA, result.Value.TaxType);
            Assert.True(result.Value.IsActive);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Create_WithBlankName_Fails(string name)
        {
            var result = Tax.Create(name, TaxType.IVA);

            Assert.True(result.IsError);
            Assert.Equal("Tax.NameIsRequired", result.FirstError.Code);
        }

        [Fact]
        public void UpdateDetails_WithValidData_UpdatesAllFields()
        {
            var tax = Tax.Create("IVA General", TaxType.IVA).Value;

            var result = tax.UpdateDetails("IVA Reducido", TaxType.IVA);

            Assert.False(result.IsError);
            Assert.Equal("IVA Reducido", tax.Name);
        }

        [Fact]
        public void UpdateDetails_WithBlankName_Fails()
        {
            var tax = Tax.Create("IVA General", TaxType.IVA).Value;

            var result = tax.UpdateDetails("", TaxType.IVA);

            Assert.True(result.IsError);
            Assert.Equal("IVA General", tax.Name);
        }

        [Fact]
        public void Deactivate_ThenActivate_TogglesIsActive()
        {
            var tax = Tax.Create("IVA General", TaxType.IVA).Value;

            tax.Deactivate();
            Assert.False(tax.IsActive);

            tax.Activate();
            Assert.True(tax.IsActive);
        }
    }
}
