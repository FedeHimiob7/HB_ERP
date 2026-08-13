using HB_ERP.SharedKernel.Domain.Primitives;
using Identity.Application.Common.Interfaces;
using Identity.Application.Roles.Commands.AssignAction;
using Identity.Domain.Entities;
using Identity.Domain.Enums;
using Identity.Domain.Repositories;
using Identity.Domain.VO;
using NSubstitute;

namespace Application.Tests
{
    public sealed class AssignActionToRoleCommandHandlerTests
    {
        private readonly IRoleRepository _roleRepository = Substitute.For<IRoleRepository>();
        private readonly IIdentityUnitOfWork _unitOfWork = Substitute.For<IIdentityUnitOfWork>();
        private readonly IEventLogRepository _eventLogRepository = Substitute.For<IEventLogRepository>();
        private readonly IFiscalClock _fiscalClock = Substitute.For<IFiscalClock>();
        private readonly ICurrentUserProvider _currentUser = Substitute.For<ICurrentUserProvider>();
        private readonly Guid _roleGuid = Guid.NewGuid();
        private readonly Guid _actionGuid = Guid.NewGuid();

        private AssignActionToRoleCommandHandler CreateHandler()
            => new(_roleRepository, _unitOfWork, _eventLogRepository, _fiscalClock, _currentUser);

        [Fact]
        public async Task Handle_WhenRoleDoesNotExist_ReturnsNotFound()
        {
            _roleRepository.GetByIdAsync(Arg.Any<RoleId>(), Arg.Any<CancellationToken>()).Returns((Role?)null);

            var result = await CreateHandler().Handle(new AssignActionToRoleCommand(_roleGuid, _actionGuid), CancellationToken.None);

            Assert.True(result.IsError);
            Assert.Equal("Role.NotFound", result.FirstError.Code);
        }

        [Fact]
        public async Task Handle_WhenValid_AssignsActionAndWritesEventLog()
        {
            var role = Role.Create("Ventas");
            _roleRepository.GetByIdAsync(Arg.Any<RoleId>(), Arg.Any<CancellationToken>()).Returns(role);
            _fiscalClock.VenezuelaNow.Returns(new DateTime(2026, 8, 10));

            var result = await CreateHandler().Handle(new AssignActionToRoleCommand(_roleGuid, _actionGuid), CancellationToken.None);

            Assert.False(result.IsError);
            Assert.Contains(new ActionsId(_actionGuid), role.ActionIds);
            await _eventLogRepository.Received(1).AddAsync(
                Arg.Is<EventLog>(e => e.Type == EventLogType.ActionAssignedToRole && e.EntityId == role.Id.Value),
                Arg.Any<CancellationToken>());
        }
    }
}
