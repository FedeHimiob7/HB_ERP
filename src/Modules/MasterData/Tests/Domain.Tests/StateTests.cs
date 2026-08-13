using MasterData.Domain.Entities;
using MasterData.Domain.VO;
using Xunit;

namespace Domain.Tests
{
    public sealed class StateTests
    {
        private static readonly CountryId ValidCountryId = CountryId.New();

        [Fact]
        public void Create_WithValidData_Succeeds()
        {
            var result = State.Create(ValidCountryId, "mi", "Miranda");

            Assert.False(result.IsError);
            // El código se normaliza a mayúsculas.
            Assert.Equal("MI", result.Value.Code);
            Assert.Equal("Miranda", result.Value.Name);
            Assert.True(result.Value.IsActive);
        }

        [Fact]
        public void Create_WithEmptyCountryId_ReturnsInvalidCountry()
        {
            var result = State.Create(CountryId.Create(Guid.Empty), "MI", "Miranda");

            Assert.True(result.IsError);
            Assert.Equal("State.InvalidCountry", result.FirstError.Code);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Create_WithBlankName_Fails(string name)
        {
            var result = State.Create(ValidCountryId, "MI", name);

            Assert.True(result.IsError);
            Assert.Equal("State.NameIsRequired", result.FirstError.Code);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Create_WithBlankCode_Fails(string code)
        {
            var result = State.Create(ValidCountryId, code, "Miranda");

            Assert.True(result.IsError);
            Assert.Equal("State.InvalidCode", result.FirstError.Code);
        }

        [Fact]
        public void UpdateDetails_WithValidData_UpdatesAllFieldsAndNormalizesCode()
        {
            var state = State.Create(ValidCountryId, "MI", "Miranda").Value;
            var newCountryId = CountryId.New();

            var result = state.UpdateDetails(newCountryId, "ar", "Aragua");

            Assert.False(result.IsError);
            Assert.Equal(newCountryId, state.CountryId);
            Assert.Equal("AR", state.Code);
            Assert.Equal("Aragua", state.Name);
        }

        [Fact]
        public void Deactivate_ThenActivate_TogglesIsActive()
        {
            var state = State.Create(ValidCountryId, "MI", "Miranda").Value;

            state.Deactivate();
            Assert.False(state.IsActive);

            state.Activate();
            Assert.True(state.IsActive);
        }
    }
}
