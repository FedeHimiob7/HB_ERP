using MasterData.Domain.Entities;
using Xunit;

namespace Domain.Tests
{
    public sealed class CurrencyTests
    {
        [Fact]
        public void Create_WithValidData_Succeeds()
        {
            var result = Currency.Create("usd", "Dólar", "$");

            Assert.False(result.IsError);
            // El código ISO se normaliza a mayúsculas.
            Assert.Equal("USD", result.Value.Code);
            Assert.Equal("Dólar", result.Value.Name);
            Assert.True(result.Value.IsActive);
        }

        [Theory]
        [InlineData("")]
        [InlineData("US")]
        [InlineData("USDD")]
        public void Create_WithCodeNotThreeCharacters_ReturnsInvalidCode(string code)
        {
            var result = Currency.Create(code, "Dólar", "$");

            Assert.True(result.IsError);
            Assert.Equal("Currency.InvalidCode", result.FirstError.Code);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Create_WithBlankName_Fails(string name)
        {
            var result = Currency.Create("USD", name, "$");

            Assert.True(result.IsError);
            Assert.Equal("Currency.NameIsRequired", result.FirstError.Code);
        }

        [Fact]
        public void UpdateDetails_WithValidData_UpdatesNameAndSymbolButNotCode()
        {
            var currency = Currency.Create("USD", "Dólar", "$").Value;

            var result = currency.UpdateDetails("Dólar Estadounidense", "US$");

            Assert.False(result.IsError);
            Assert.Equal("Dólar Estadounidense", currency.Name);
            Assert.Equal("US$", currency.Symbol);
            Assert.Equal("USD", currency.Code);
        }

        [Fact]
        public void UpdateDetails_WithBlankName_Fails()
        {
            var currency = Currency.Create("USD", "Dólar", "$").Value;

            var result = currency.UpdateDetails("", "US$");

            Assert.True(result.IsError);
            Assert.Equal("Dólar", currency.Name);
        }

        [Fact]
        public void Deactivate_ThenActivate_TogglesIsActive()
        {
            var currency = Currency.Create("USD", "Dólar", "$").Value;

            currency.Deactivate();
            Assert.False(currency.IsActive);

            currency.Activate();
            Assert.True(currency.IsActive);
        }
    }
}
