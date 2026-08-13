using HB_ERP.SharedKernel.Domain.Primitives;
using Identity.Application.Common.Interfaces;
using Identity.Application.Roles.Commands.DeleteRole;
using Identity.Domain;
using Identity.Domain.DomainErrors;
using Identity.Domain.Entities;
using Identity.Domain.Enums;
using Identity.Domain.Repositories;
using Identity.Domain.VO;
using NSubstitute;

namespace Application.Tests
{
    public sealed class DeleteRoleCommandHandlerTests
    {
        private readonly IRoleRepository _roleRepository = Substitute.For<IRoleRepository>();
        private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
        private readonly IIdentityUnitOfWork _unitOfWork = Substitute.For<IIdentityUnitOfWork>();
        private readonly IEventLogRepository _eventLogRepository = Substitute.For<IEventLogRepository>();
        private readonly IFiscalClock _fiscalClock = Substitute.For<IFiscalClock>();
        private readonly ICurrentUserProvider _currentUser = Substitute.For<ICurrentUserProvider>();
        private readonly Guid _roleGuid = Guid.NewGuid();

        private DeleteRoleCommandHandler CreateHandler()
            => new(_roleRepository, _userRepository, _unitOfWork, _eventLogRepository, _fiscalClock, _currentUser);

        [Fact]
        public async Task Handle_WhenRoleDoesNotExist_ReturnsNotFound()
        {
            _roleRepository.GetByIdAsync(Arg.Any<RoleId>(), Arg.Any<CancellationToken>()).Returns((Role?)null);

            var result = await CreateHandler().Handle(new DeleteRoleCommand(_roleGuid), CancellationToken.None);

            Assert.True(result.IsError);
            Assert.Equal(RoleErrors.RoleNotFound.Code, result.FirstError.Code);
        }

        [Fact]
        public async Task Handle_WhenValid_RemovesRoleFromAffectedUsersAndWritesEventLogWithCount()
        {
            // Patrón de cleanup en el handler (no domain event handler — ver nota en CLAUDE.md
            // sobre PublishDomainEventsInterceptor en Identity): al desactivar un Role, el propio
            // handler busca los usuarios que lo tienen y les revoca el rol en la misma transacción.
            var role = Role.Create("Ventas");
            _roleRepository.GetByIdAsync(Arg.Any<RoleId>(), Arg.Any<CancellationToken>()).Returns(role);

            var affectedUser = Identity.Domain.User.Register(
                "Juan", "Perez", Identity.Domain.VO.Email.Create("juan@ejemplo.com").Value, Identity.Domain.VO.PasswordHash.Create("hash"));
            affectedUser.AssignRole(new RoleId(_roleGuid));
            _userRepository.GetUsersByRoleIdAsync(_roleGuid, Arg.Any<CancellationToken>())
                .Returns(new List<Identity.Domain.User> { affectedUser });

            _fiscalClock.VenezuelaNow.Returns(new DateTime(2026, 8, 10));

            var result = await CreateHandler().Handle(new DeleteRoleCommand(_roleGuid), CancellationToken.None);

            Assert.False(result.IsError);
            Assert.False(role.IsActive);
            Assert.DoesNotContain(new RoleId(_roleGuid), affectedUser.Roles);
            await _userRepository.Received(1).UpdateAsync(affectedUser);

            await _eventLogRepository.Received(1).AddAsync(
                Arg.Is<EventLog>(e => e.Type == EventLogType.RoleDeactivated && e.Description.Contains("1 usuario")),
                Arg.Any<CancellationToken>());
        }
    }
}
