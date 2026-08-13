using Identity.Domain.Entities;
using Identity.Domain.VO;
using Xunit;

namespace Domain.Tests
{
    public sealed class RoleTests
    {
        [Fact]
        public void Create_WithValidName_CreatesActiveRole()
        {
            var role = Role.Create("Ventas");

            Assert.Equal("Ventas", role.Name);
            Assert.True(role.IsActive);
            Assert.Empty(role.ActionIds);
        }

        [Fact]
        public void AssignAction_WhenNotAlreadyAssigned_AddsAction()
        {
            var role = Role.Create("Ventas");
            var actionId = new ActionsId(Guid.NewGuid());

            role.AssignAction(actionId);

            Assert.Contains(actionId, role.ActionIds);
        }

        [Fact]
        public void AssignAction_WhenAlreadyAssigned_DoesNotDuplicate()
        {
            var role = Role.Create("Ventas");
            var actionId = new ActionsId(Guid.NewGuid());
            role.AssignAction(actionId);

            role.AssignAction(actionId);

            Assert.Single(role.ActionIds);
        }

        [Fact]
        public void RevokeAction_WhenAssigned_RemovesAction()
        {
            var role = Role.Create("Ventas");
            var actionId = new ActionsId(Guid.NewGuid());
            role.AssignAction(actionId);

            role.RevokeAction(actionId);

            Assert.Empty(role.ActionIds);
        }

        [Fact]
        public void SyncActions_AddsNewAndRemovesMissing()
        {
            var role = Role.Create("Ventas");
            var actionToKeep = new ActionsId(Guid.NewGuid());
            var actionToRemove = new ActionsId(Guid.NewGuid());
            var actionToAdd = new ActionsId(Guid.NewGuid());
            role.AssignAction(actionToKeep);
            role.AssignAction(actionToRemove);

            role.SyncActions(new[] { actionToKeep, actionToAdd });

            Assert.Contains(actionToKeep, role.ActionIds);
            Assert.Contains(actionToAdd, role.ActionIds);
            Assert.DoesNotContain(actionToRemove, role.ActionIds);
        }

        [Fact]
        public void ChangeName_UpdatesName()
        {
            var role = Role.Create("Ventas");

            role.ChangeName("Ventas al mayor");

            Assert.Equal("Ventas al mayor", role.Name);
        }

        [Fact]
        public void Deactivate_ThenActivate_TogglesIsActive()
        {
            var role = Role.Create("Ventas");

            role.Deactivate();
            Assert.False(role.IsActive);

            role.Activate();
            Assert.True(role.IsActive);
        }
    }
}
