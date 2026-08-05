using MasterData.Domain.Entities;
using MasterData.Domain.Enums;
using MasterData.Domain.VO;
using Xunit;

namespace Domain.Tests
{
    public sealed class CompanyTests
    {
        [Fact]
        public void Create_WithValidData_Succeeds()
        {
            var result = Company.Create("J-401027631-4", "LA Export Group", "Av. Principal, Caracas", TaxpayerType.Ordinario);

            Assert.False(result.IsError);
            Assert.Equal("J-401027631-4", result.Value.Rif);
            Assert.Equal("LA Export Group", result.Value.LegalName);
            Assert.Equal(CompanyId.Singleton, result.Value.Id);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Create_WithBlankRif_Fails(string rif)
        {
            var result = Company.Create(rif, "LA Export Group", "Av. Principal, Caracas", TaxpayerType.Ordinario);

            Assert.True(result.IsError);
            Assert.Equal("Company.RifIsRequired", result.FirstError.Code);
        }

        [Theory]
        [InlineData("12345678")]
        [InlineData("J-12345678")]
        [InlineData("X-12345678-9")]
        [InlineData("J-123-9")]
        public void Create_WithInvalidRifFormat_Fails(string rif)
        {
            var result = Company.Create(rif, "LA Export Group", "Av. Principal, Caracas", TaxpayerType.Ordinario);

            Assert.True(result.IsError);
            Assert.Equal("Company.InvalidRifFormat", result.FirstError.Code);
        }

        [Fact]
        public void Create_WithBlankLegalName_Fails()
        {
            var result = Company.Create("J-401027631-4", "", "Av. Principal, Caracas", TaxpayerType.Ordinario);

            Assert.True(result.IsError);
            Assert.Equal("Company.LegalNameIsRequired", result.FirstError.Code);
        }

        [Fact]
        public void Create_WithBlankRegisteredAddress_Fails()
        {
            var result = Company.Create("J-401027631-4", "LA Export Group", "", TaxpayerType.Ordinario);

            Assert.True(result.IsError);
            Assert.Equal("Company.RegisteredAddressIsRequired", result.FirstError.Code);
        }

        [Fact]
        public void UpdateDetails_WithValidData_UpdatesAllFields()
        {
            var company = Company.Create("J-401027631-4", "LA Export Group", "Av. Principal, Caracas", TaxpayerType.Ordinario).Value;

            var result = company.UpdateDetails("V-12345678-9", "LA Zapatería", "Av. Bolívar, Valencia", TaxpayerType.Especial);

            Assert.False(result.IsError);
            Assert.Equal("V-12345678-9", company.Rif);
            Assert.Equal("LA Zapatería", company.LegalName);
            Assert.Equal("Av. Bolívar, Valencia", company.RegisteredAddress);
            Assert.Equal(TaxpayerType.Especial, company.TaxpayerType);
        }

        [Fact]
        public void UpdateDetails_WithInvalidRif_Fails()
        {
            var company = Company.Create("J-401027631-4", "LA Export Group", "Av. Principal, Caracas", TaxpayerType.Ordinario).Value;

            var result = company.UpdateDetails("invalid", "LA Zapatería", "Av. Bolívar, Valencia", TaxpayerType.Especial);

            Assert.True(result.IsError);
            Assert.Equal("J-401027631-4", company.Rif);
        }
    }
}
