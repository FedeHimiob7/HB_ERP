using HB_ERP.SharedKernel.Domain.Primitives;
using Identity.Application.Common.Interfaces;
using Identity.Application.SystemActions.Commands.DeleteSystemAction;
using Identity.Domain.DomainErrors;
using Identity.Domain.Entities;
using Identity.Domain.Enums;
using Identity.Domain.Repositories;
using Identity.Domain.VO;
using NSubstitute;

namespace Application.Tests
{
    public sealed class DeleteSystemActionCommandHandlerTests
    {
        private readonly ISystemActionRepository _systemActionRepository = Substitute.For<ISystemActionRepository>();
        private readonly IRoleRepository _roleRepository = Substitute.For<IRoleRepository>();
        private readonly IIdentityUnitOfWork _unitOfWork = Substitute.For<IIdentityUnitOfWork>();
        private readonly IEventLogRepository _eventLogRepository = Substitute.For<IEventLogRepository>();
        private readonly IFiscalClock _fiscalClock = Substitute.For<IFiscalClock>();
        private readonly ICurrentUserProvider _currentUser = Substitute.For<ICurrentUserProvider>();
        private readonly Guid _actionGuid = Guid.NewGuid();

        private DeleteSystemActionCommandHandler CreateHandler()
            => new(_systemActionRepository, _roleRepository, _unitOfWork, _eventLogRepository, _fiscalClock, _currentUser);

        [Fact]
        public async Task Handle_WhenActionDoesNotExist_ReturnsNotFound()
        {
            _systemActionRepository.GetByIdAsync(Arg.Any<ActionsId>()).Returns((SystemAction?)null);

            var result = await CreateHandler().Handle(new DeleteSystemActionCommand(_actionGuid), CancellationToken.None);

            Assert.True(result.IsError);
            Assert.Equal(SystemActionErrors.NotFound.Code, result.FirstError.Code);
        }

        [Fact]
        public async Task Handle_WhenValid_RevokesActionFromAffectedRolesAndWritesEventLogWithCount()
        {
            var action = SystemAction.Create("products.create", "Crear productos");
            _systemActionRepository.GetByIdAsync(Arg.Any<ActionsId>()).Returns(action);

            var affectedRole = Identity.Domain.Entities.Role.Create("Ventas");
            affectedRole.AssignAction(new ActionsId(_actionGuid));
            _roleRepository.GetRolesByActionIdAsync(_actionGuid, Arg.Any<CancellationToken>())
                .Returns(new List<Identity.Domain.Entities.Role> { affectedRole });

            _fiscalClock.VenezuelaNow.Returns(new DateTime(2026, 8, 10));

            var result = await CreateHandler().Handle(new DeleteSystemActionCommand(_actionGuid), CancellationToken.None);

            Assert.False(result.IsError);
            Assert.False(action.IsActive);
            Assert.DoesNotContain(new ActionsId(_actionGuid), affectedRole.ActionIds);
            await _roleRepository.Received(1).UpdateAsync(affectedRole, Arg.Any<CancellationToken>());

            await _eventLogRepository.Received(1).AddAsync(
                Arg.Is<EventLog>(e => e.Type == EventLogType.SystemActionDeactivated && e.Description.Contains("1 rol")),
                Arg.Any<CancellationToken>());
        }
    }
}
