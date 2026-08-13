using HB_ERP.SharedKernel.Domain.Primitives;
using Identity.Application.Common.Interfaces;
using Identity.Application.SystemActions.Commands.UpdateSystemAction;
using Identity.Domain.DomainErrors;
using Identity.Domain.Entities;
using Identity.Domain.Enums;
using Identity.Domain.Repositories;
using Identity.Domain.VO;
using NSubstitute;

namespace Application.Tests
{
    public sealed class UpdateSystemActionCommandHandlerTests
    {
        private readonly ISystemActionRepository _repository = Substitute.For<ISystemActionRepository>();
        private readonly IIdentityUnitOfWork _unitOfWork = Substitute.For<IIdentityUnitOfWork>();
        private readonly IEventLogRepository _eventLogRepository = Substitute.For<IEventLogRepository>();
        private readonly IFiscalClock _fiscalClock = Substitute.For<IFiscalClock>();
        private readonly ICurrentUserProvider _currentUser = Substitute.For<ICurrentUserProvider>();
        private readonly Guid _actionGuid = Guid.NewGuid();

        private UpdateSystemActionCommandHandler CreateHandler()
            => new(_repository, _unitOfWork, _eventLogRepository, _fiscalClock, _currentUser);

        [Fact]
        public async Task Handle_WhenActionDoesNotExist_ReturnsNotFound()
        {
            _repository.GetByIdAsync(Arg.Any<ActionsId>()).Returns((SystemAction?)null);

            var command = new UpdateSystemActionCommand(_actionGuid, "products.create", "Crear productos");
            var result = await CreateHandler().Handle(command, CancellationToken.None);

            Assert.True(result.IsError);
            Assert.Equal(SystemActionErrors.NotFound.Code, result.FirstError.Code);
        }

        [Fact]
        public async Task Handle_WhenValid_WritesSystemActionUpdatedEventLog()
        {
            var action = SystemAction.Create("products.create", "Crear productos");
            _repository.GetByIdAsync(Arg.Any<ActionsId>()).Returns(action);
            _fiscalClock.VenezuelaNow.Returns(new DateTime(2026, 8, 10));

            var command = new UpdateSystemActionCommand(_actionGuid, "products.create", "Crear productos nuevos");
            var result = await CreateHandler().Handle(command, CancellationToken.None);

            Assert.False(result.IsError);
            Assert.Equal("Crear productos nuevos", action.Description);
            await _eventLogRepository.Received(1).AddAsync(
                Arg.Is<EventLog>(e => e.Type == EventLogType.SystemActionUpdated && e.EntityId == action.Id.Value),
                Arg.Any<CancellationToken>());
        }
    }
}
