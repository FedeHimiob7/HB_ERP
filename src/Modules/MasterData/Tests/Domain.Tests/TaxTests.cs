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
            var result = Tax.Create("IVA General", TaxType.IVA, 16m);

            Assert.False(result.IsError);
            Assert.Equal("IVA General", result.Value.Name);
            Assert.Equal(TaxType.IVA, result.Value.TaxType);
            Assert.Equal(16m, result.Value.Rate);
            Assert.True(result.Value.IsActive);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Create_WithBlankName_Fails(string name)
        {
            var result = Tax.Create(name, TaxType.IVA, 16m);

            Assert.True(result.IsError);
            Assert.Equal("Tax.NameIsRequired", result.FirstError.Code);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Create_WithNonPositiveRate_Fails(decimal rate)
        {
            var result = Tax.Create("IGTF", TaxType.IGTF, rate);

            Assert.True(result.IsError);
            Assert.Equal("Tax.RateMustBePositive", result.FirstError.Code);
        }

        [Fact]
        public void UpdateDetails_WithValidData_UpdatesAllFields()
        {
            var tax = Tax.Create("IVA General", TaxType.IVA, 16m).Value;

            var result = tax.UpdateDetails("IVA Reducido", TaxType.IVA, 8m);

            Assert.False(result.IsError);
            Assert.Equal("IVA Reducido", tax.Name);
            Assert.Equal(8m, tax.Rate);
        }

        [Fact]
        public void UpdateDetails_WithNonPositiveRate_Fails()
        {
            var tax = Tax.Create("IVA General", TaxType.IVA, 16m).Value;

            var result = tax.UpdateDetails("IVA General", TaxType.IVA, 0m);

            Assert.True(result.IsError);
            Assert.Equal(16m, tax.Rate);
        }

        [Fact]
        public void Deactivate_ThenActivate_TogglesIsActive()
        {
            var tax = Tax.Create("IVA General", TaxType.IVA, 16m).Value;

            tax.Deactivate();
            Assert.False(tax.IsActive);

            tax.Activate();
            Assert.True(tax.IsActive);
        }
    }
}
