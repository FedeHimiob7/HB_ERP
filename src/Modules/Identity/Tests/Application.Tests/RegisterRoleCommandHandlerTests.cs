using HB_ERP.SharedKernel.Domain.Primitives;
using Identity.Application.Common.Interfaces;
using Identity.Application.Roles.Commands.RegisterRole;
using Identity.Domain;
using Identity.Domain.DomainErrors;
using Identity.Domain.Entities;
using Identity.Domain.Enums;
using Identity.Domain.Repositories;
using NSubstitute;

namespace Application.Tests
{
    public sealed class RegisterRoleCommandHandlerTests
    {
        private readonly IRoleRepository _roleRepository = Substitute.For<IRoleRepository>();
        private readonly ISystemActionRepository _systemActionRepository = Substitute.For<ISystemActionRepository>();
        private readonly IIdentityUnitOfWork _unitOfWork = Substitute.For<IIdentityUnitOfWork>();
        private readonly IRoleNameUniquenessChecker _roleNameChecker = Substitute.For<IRoleNameUniquenessChecker>();
        private readonly IEventLogRepository _eventLogRepository = Substitute.For<IEventLogRepository>();
        private readonly IFiscalClock _fiscalClock = Substitute.For<IFiscalClock>();
        private readonly ICurrentUserProvider _currentUser = Substitute.For<ICurrentUserProvider>();

        private RegisterRoleCommandHandler CreateHandler()
            => new(_roleRepository, _systemActionRepository, _unitOfWork, _roleNameChecker, _eventLogRepository, _fiscalClock, _currentUser);

        [Fact]
        public async Task Handle_WhenNameAlreadyInUse_ReturnsErrorWithoutWritingEventLog()
        {
            _roleNameChecker.IsRoleNameUniqueAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(false);

            var result = await CreateHandler().Handle(new RegisterRoleCommand("Ventas", null), CancellationToken.None);

            Assert.True(result.IsError);
            Assert.Equal(RoleErrors.NameAlreadyInUse.Code, result.FirstError.Code);
            await _eventLogRepository.DidNotReceive().AddAsync(Arg.Any<EventLog>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_WhenValid_WritesRoleCreatedEventLog()
        {
            _roleNameChecker.IsRoleNameUniqueAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(true);
            _fiscalClock.VenezuelaNow.Returns(new DateTime(2026, 8, 10));

            var result = await CreateHandler().Handle(new RegisterRoleCommand("Ventas", null), CancellationToken.None);

            Assert.False(result.IsError);
            await _eventLogRepository.Received(1).AddAsync(
                Arg.Is<EventLog>(e => e.Type == EventLogType.RoleCreated && e.EntityType == nameof(Role) && e.EntityId == result.Value),
                Arg.Any<CancellationToken>());
        }
    }
}
