using HB_ERP.SharedKernel.Domain.Primitives;
using Identity.Application.Common.Interfaces;
using Identity.Application.SystemActions.Commands.Create;
using Identity.Domain.DomainErrors;
using Identity.Domain.Entities;
using Identity.Domain.Enums;
using Identity.Domain.Repositories;
using NSubstitute;

namespace Application.Tests
{
    public sealed class CreateSystemActionCommandHandlerTests
    {
        private readonly ISystemActionRepository _repository = Substitute.For<ISystemActionRepository>();
        private readonly IIdentityUnitOfWork _unitOfWork = Substitute.For<IIdentityUnitOfWork>();
        private readonly IEventLogRepository _eventLogRepository = Substitute.For<IEventLogRepository>();
        private readonly IFiscalClock _fiscalClock = Substitute.For<IFiscalClock>();
        private readonly ICurrentUserProvider _currentUser = Substitute.For<ICurrentUserProvider>();

        private CreateSystemActionCommandHandler CreateHandler()
            => new(_repository, _unitOfWork, _eventLogRepository, _fiscalClock, _currentUser);

        [Fact]
        public async Task Handle_WhenNameNotUnique_ReturnsDuplicateName()
        {
            _repository.IsNameUniqueAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(false);

            var result = await CreateHandler().Handle(new CreateSystemActionCommand("products.create", "Crear productos"), CancellationToken.None);

            Assert.True(result.IsError);
            Assert.Equal(SystemActionErrors.DuplicateName.Code, result.FirstError.Code);
        }

        [Fact]
        public async Task Handle_WhenValid_WritesSystemActionCreatedEventLog()
        {
            _repository.IsNameUniqueAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(true);
            _fiscalClock.VenezuelaNow.Returns(new DateTime(2026, 8, 10));

            var result = await CreateHandler().Handle(new CreateSystemActionCommand("products.create", "Crear productos"), CancellationToken.None);

            Assert.False(result.IsError);
            await _eventLogRepository.Received(1).AddAsync(
                Arg.Is<EventLog>(e => e.Type == EventLogType.SystemActionCreated && e.EntityType == nameof(SystemAction) && e.EntityId == result.Value),
                Arg.Any<CancellationToken>());
        }
    }
}
