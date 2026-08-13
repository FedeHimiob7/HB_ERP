using HB_ERP.SharedKernel.Domain.Primitives;
using Identity.Application.Common.Interfaces;
using Identity.Application.Users.Commands.UpdateUser;
using Identity.Domain;
using Identity.Domain.Entities;
using Identity.Domain.Enums;
using Identity.Domain.VO;
using NSubstitute;

namespace Application.Tests
{
    public sealed class UpdateUserCommandHandlerTests
    {
        private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
        private readonly IIdentityUnitOfWork _unitOfWork = Substitute.For<IIdentityUnitOfWork>();
        private readonly IUserEmailUniquenessChecker _emailChecker = Substitute.For<IUserEmailUniquenessChecker>();
        private readonly IPslExistenceChecker _pslChecker = Substitute.For<IPslExistenceChecker>();
        private readonly Identity.Domain.Repositories.IEventLogRepository _eventLogRepository = Substitute.For<Identity.Domain.Repositories.IEventLogRepository>();
        private readonly IFiscalClock _fiscalClock = Substitute.For<IFiscalClock>();
        private readonly ICurrentUserProvider _currentUser = Substitute.For<ICurrentUserProvider>();
        private readonly Guid _userGuid = Guid.NewGuid();

        private UpdateUserCommandHandler CreateHandler()
            => new(_userRepository, _unitOfWork, _emailChecker, _pslChecker, _eventLogRepository, _fiscalClock, _currentUser);

        [Fact]
        public async Task Handle_WhenValid_WritesUserUpdatedEventLog()
        {
            var user = User.Register("Juan", "Perez", Email.Create("juan@ejemplo.com").Value, PasswordHash.Create("hash"));
            _userRepository.GetByIdAsync(Arg.Any<UserId>()).Returns(user);
            _fiscalClock.VenezuelaNow.Returns(new DateTime(2026, 8, 10));

            // Mismo email que ya tenía el usuario, para no disparar el chequeo de unicidad.
            var command = new UpdateUserCommand(_userGuid, "Juan Carlos", "Perez", "juan@ejemplo.com", []);
            var result = await CreateHandler().Handle(command, CancellationToken.None);

            Assert.False(result.IsError);
            Assert.Equal("Juan Carlos", user.FirstName);
            await _eventLogRepository.Received(1).AddAsync(
                Arg.Is<EventLog>(e => e.Type == EventLogType.UserUpdated && e.EntityId == user.Id.Value),
                Arg.Any<CancellationToken>());
        }
    }
}
