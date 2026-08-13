using MasterData.Domain.Entities;
using Xunit;

namespace Domain.Tests
{
    public sealed class CountryTests
    {
        [Fact]
        public void Create_WithValidName_Succeeds()
        {
            var result = Country.Create("Venezuela");

            Assert.False(result.IsError);
            Assert.Equal("Venezuela", result.Value.Name);
            Assert.True(result.Value.IsActive);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Create_WithBlankName_Fails(string name)
        {
            var result = Country.Create(name);

            Assert.True(result.IsError);
            Assert.Equal("Country.NameIsRequired", result.FirstError.Code);
        }

        [Fact]
        public void UpdateDetails_WithValidName_UpdatesName()
        {
            var country = Country.Create("Venezuela").Value;

            var result = country.UpdateDetails("Colombia");

            Assert.False(result.IsError);
            Assert.Equal("Colombia", country.Name);
        }

        [Fact]
        public void UpdateDetails_WithBlankName_Fails()
        {
            var country = Country.Create("Venezuela").Value;

            var result = country.UpdateDetails("");

            Assert.True(result.IsError);
            Assert.Equal("Venezuela", country.Name);
        }

        [Fact]
        public void Deactivate_ThenActivate_TogglesIsActive()
        {
            var country = Country.Create("Venezuela").Value;

            country.Deactivate();
            Assert.False(country.IsActive);

            country.Activate();
            Assert.True(country.IsActive);
        }
    }
}
