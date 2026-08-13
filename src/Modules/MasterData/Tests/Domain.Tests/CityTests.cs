using MasterData.Domain.Entities;
using MasterData.Domain.VO;
using Xunit;

namespace Domain.Tests
{
    public sealed class CityTests
    {
        private static readonly StateId ValidStateId = StateId.New();

        [Fact]
        public void Create_WithValidData_Succeeds()
        {
            var result = City.Create(ValidStateId, "Los Teques");

            Assert.False(result.IsError);
            Assert.Equal("Los Teques", result.Value.Name);
            Assert.True(result.Value.IsActive);
        }

        [Fact]
        public void Create_WithEmptyStateId_ReturnsInvalidState()
        {
            var result = City.Create(StateId.Create(Guid.Empty), "Los Teques");

            Assert.True(result.IsError);
            Assert.Equal("City.InvalidState", result.FirstError.Code);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Create_WithBlankName_Fails(string name)
        {
            var result = City.Create(ValidStateId, name);

            Assert.True(result.IsError);
            Assert.Equal("City.NameIsRequired", result.FirstError.Code);
        }

        [Fact]
        public void UpdateDetails_WithValidData_UpdatesAllFields()
        {
            var city = City.Create(ValidStateId, "Los Teques").Value;
            var newStateId = StateId.New();

            var result = city.UpdateDetails(newStateId, "San Antonio");

            Assert.False(result.IsError);
            Assert.Equal(newStateId, city.StateId);
            Assert.Equal("San Antonio", city.Name);
        }

        [Fact]
        public void Deactivate_ThenActivate_TogglesIsActive()
        {
            var city = City.Create(ValidStateId, "Los Teques").Value;

            city.Deactivate();
            Assert.False(city.IsActive);

            city.Activate();
            Assert.True(city.IsActive);
        }
    }
}
