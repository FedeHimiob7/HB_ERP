using Identity.Domain.Entities;
using Identity.Domain.VO;
using Xunit;

namespace Domain.Tests
{
    public sealed class SystemActionTests
    {
        [Fact]
        public void Create_WithValidData_CreatesActiveSystemAction()
        {
            var action = SystemAction.Create("products.create", "Crear productos");

            Assert.Equal("products.create", action.Name.Value);
            Assert.Equal("Crear productos", action.Description);
            Assert.True(action.IsActive);
        }

        [Fact]
        public void UpdateDetails_WithValidData_UpdatesNameAndDescription()
        {
            var action = SystemAction.Create("products.create", "Crear productos");
            var newName = ActionName.Create("products.create.v2");

            action.UpdateDetails(newName, "Crear productos version 2");

            Assert.Equal(newName, action.Name);
            Assert.Equal("Crear productos version 2", action.Description);
        }

        [Fact]
        public void Deactivate_WhenActive_SetsIsActiveFalse()
        {
            var action = SystemAction.Create("products.create", "Crear productos");

            action.Deactivate();

            Assert.False(action.IsActive);
        }

        [Fact]
        public void Deactivate_WhenAlreadyInactive_IsIdempotent()
        {
            var action = SystemAction.Create("products.create", "Crear productos");
            action.Deactivate();

            action.Deactivate();

            Assert.False(action.IsActive);
        }

        [Fact]
        public void Activate_ThenDeactivate_TogglesIsActive()
        {
            var action = SystemAction.Create("products.create", "Crear productos");
            action.Deactivate();

            action.Activate();
            Assert.True(action.IsActive);

            action.Deactivate();
            Assert.False(action.IsActive);
        }
    }
}
