using HB_ERP.SharedKernel.Domain.Primitives;
using Identity.Application.Common.Interfaces;
using Identity.Application.Roles.Commands.UpdateRole;
using Identity.Domain;
using Identity.Domain.DomainErrors;
using Identity.Domain.Entities;
using Identity.Domain.Enums;
using Identity.Domain.Repositories;
using Identity.Domain.VO;
using NSubstitute;

namespace Application.Tests
{
    public sealed class UpdateRoleCommandHandlerTests
    {
        private readonly IRoleRepository _roleRepository = Substitute.For<IRoleRepository>();
        private readonly ISystemActionRepository _systemActionRepository = Substitute.For<ISystemActionRepository>();
        private readonly IIdentityUnitOfWork _unitOfWork = Substitute.For<IIdentityUnitOfWork>();
        private readonly IRoleNameUniquenessChecker _roleNameChecker = Substitute.For<IRoleNameUniquenessChecker>();
        private readonly IEventLogRepository _eventLogRepository = Substitute.For<IEventLogRepository>();
        private readonly IFiscalClock _fiscalClock = Substitute.For<IFiscalClock>();
        private readonly ICurrentUserProvider _currentUser = Substitute.For<ICurrentUserProvider>();
        private readonly Guid _roleGuid = Guid.NewGuid();

        private UpdateRoleCommandHandler CreateHandler()
            => new(_roleRepository, _systemActionRepository, _unitOfWork, _roleNameChecker, _eventLogRepository, _fiscalClock, _currentUser);

        [Fact]
        public async Task Handle_WhenRoleDoesNotExist_ReturnsNotFound()
        {
            _roleRepository.GetByIdAsync(Arg.Any<RoleId>(), Arg.Any<CancellationToken>()).Returns((Role?)null);

            var result = await CreateHandler().Handle(new UpdateRoleCommand(_roleGuid, "Ventas", null), CancellationToken.None);

            Assert.True(result.IsError);
            Assert.Equal(RoleErrors.RoleNotFound.Code, result.FirstError.Code);
        }

        [Fact]
        public async Task Handle_WhenNameUnchanged_SkipsUniquenessCheckAndWritesRoleUpdatedEventLog()
        {
            // El chequeo de unicidad es case-insensitive: mismo nombre (distinto casing) no debe
            // disparar la validación de duplicado.
            var role = Role.Create("Ventas");
            _roleRepository.GetByIdAsync(Arg.Any<RoleId>(), Arg.Any<CancellationToken>()).Returns(role);
            _fiscalClock.VenezuelaNow.Returns(new DateTime(2026, 8, 10));

            var result = await CreateHandler().Handle(new UpdateRoleCommand(_roleGuid, "VENTAS", null), CancellationToken.None);

            Assert.False(result.IsError);
            await _roleNameChecker.DidNotReceive().IsRoleNameUniqueAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
            await _eventLogRepository.Received(1).AddAsync(
                Arg.Is<EventLog>(e => e.Type == EventLogType.RoleUpdated && e.EntityId == role.Id.Value),
                Arg.Any<CancellationToken>());
        }
    }
}
