using Identity.Domain;
using Identity.Domain.VO;
using Xunit;

namespace Domain.Tests
{
    public sealed class UserTests
    {
        private static Email ValidEmail => Email.Create("juan@ejemplo.com").Value;
        private static PasswordHash ValidPasswordHash => PasswordHash.Create("hash");

        [Fact]
        public void Register_WithValidData_CreatesActiveUser()
        {
            var user = User.Register("Juan", "Perez", ValidEmail, ValidPasswordHash);

            Assert.Equal("Juan", user.FirstName);
            Assert.Equal("Perez", user.LastName);
            Assert.True(user.IsActive);
            Assert.Empty(user.Roles);
            Assert.Empty(user.Psls);
        }

        [Fact]
        public void Deactivate_WhenActive_SetsIsActiveFalse()
        {
            // Regresión: Deactivate() tenía la condición invertida (if (IsActive) return;) y nunca
            // desactivaba a un usuario activo — corregido junto con este test.
            var user = User.Register("Juan", "Perez", ValidEmail, ValidPasswordHash);

            user.Deactivate();

            Assert.False(user.IsActive);
        }

        [Fact]
        public void Deactivate_WhenAlreadyInactive_IsIdempotent()
        {
            var user = User.Register("Juan", "Perez", ValidEmail, ValidPasswordHash);
            user.Deactivate();

            user.Deactivate();

            Assert.False(user.IsActive);
        }

        [Fact]
        public void Activate_ThenDeactivate_TogglesIsActive()
        {
            var user = User.Register("Juan", "Perez", ValidEmail, ValidPasswordHash);
            user.Deactivate();

            user.Activate();
            Assert.True(user.IsActive);

            user.Deactivate();
            Assert.False(user.IsActive);
        }

        [Fact]
        public void AssignRole_WhenNotAlreadyAssigned_AddsRole()
        {
            var user = User.Register("Juan", "Perez", ValidEmail, ValidPasswordHash);
            var roleId = RoleId.New();

            user.AssignRole(roleId);

            Assert.Contains(roleId, user.Roles);
        }

        [Fact]
        public void AssignRole_WhenAlreadyAssigned_DoesNotDuplicate()
        {
            var user = User.Register("Juan", "Perez", ValidEmail, ValidPasswordHash);
            var roleId = RoleId.New();
            user.AssignRole(roleId);

            user.AssignRole(roleId);

            Assert.Single(user.Roles);
        }

        [Fact]
        public void RemoveRole_WhenAssigned_RemovesRole()
        {
            var user = User.Register("Juan", "Perez", ValidEmail, ValidPasswordHash);
            var roleId = RoleId.New();
            user.AssignRole(roleId);

            user.RemoveRole(roleId);

            Assert.Empty(user.Roles);
        }

        [Fact]
        public void SyncRoles_AddsNewAndRemovesMissing()
        {
            var user = User.Register("Juan", "Perez", ValidEmail, ValidPasswordHash);
            var roleToKeep = RoleId.New();
            var roleToRemove = RoleId.New();
            var roleToAdd = RoleId.New();
            user.AssignRole(roleToKeep);
            user.AssignRole(roleToRemove);

            user.SyncRoles(new[] { roleToKeep, roleToAdd });

            Assert.Contains(roleToKeep, user.Roles);
            Assert.Contains(roleToAdd, user.Roles);
            Assert.DoesNotContain(roleToRemove, user.Roles);
        }

        [Fact]
        public void SyncPsls_AddsNewAndRemovesMissing()
        {
            var user = User.Register("Juan", "Perez", ValidEmail, ValidPasswordHash);
            var pslToKeep = PslId.New();
            var pslToRemove = PslId.New();
            var pslToAdd = PslId.New();
            user.AssignPsl(pslToKeep);
            user.AssignPsl(pslToRemove);

            user.SyncPsls(new[] { pslToKeep, pslToAdd });

            Assert.Contains(pslToKeep, user.Psls);
            Assert.Contains(pslToAdd, user.Psls);
            Assert.DoesNotContain(pslToRemove, user.Psls);
        }

        [Fact]
        public void ChangeEmail_UpdatesEmail()
        {
            var user = User.Register("Juan", "Perez", ValidEmail, ValidPasswordHash);
            var newEmail = Email.Create("juan.perez@ejemplo.com").Value;

            user.ChangeEmail(newEmail);

            Assert.Equal(newEmail, user.Email);
        }

        [Fact]
        public void ChangePassword_UpdatesPasswordHash()
        {
            var user = User.Register("Juan", "Perez", ValidEmail, ValidPasswordHash);
            var newHash = PasswordHash.Create("hash-nuevo");

            user.ChangePassword(newHash);

            Assert.Equal(newHash, user.PasswordHash);
        }
    }
}
