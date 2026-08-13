using HB_ERP.SharedKernel.Domain.Primitives;
using Identity.Application.Common.Interfaces;
using Identity.Application.Users.Commands.DeleteUser;
using Identity.Domain;
using Identity.Domain.Entities;
using Identity.Domain.Enums;
using Identity.Domain.VO;
using NSubstitute;

namespace Application.Tests
{
    public sealed class DeleteUserCommandHandlerTests
    {
        private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
        private readonly IIdentityUnitOfWork _unitOfWork = Substitute.For<IIdentityUnitOfWork>();
        private readonly Identity.Domain.Repositories.IEventLogRepository _eventLogRepository = Substitute.For<Identity.Domain.Repositories.IEventLogRepository>();
        private readonly IFiscalClock _fiscalClock = Substitute.For<IFiscalClock>();
        private readonly ICurrentUserProvider _currentUser = Substitute.For<ICurrentUserProvider>();
        private readonly Guid _userGuid = Guid.NewGuid();

        private DeleteUserCommandHandler CreateHandler()
            => new(_userRepository, _unitOfWork, _eventLogRepository, _fiscalClock, _currentUser);

        [Fact]
        public async Task Handle_WhenValid_WritesUserDeactivatedEventLogWithActingUser()
        {
            var user = User.Register("Juan", "Perez", Email.Create("juan@ejemplo.com").Value, PasswordHash.Create("hash"));
            _userRepository.GetByIdAsync(Arg.Any<UserId>()).Returns(user);
            var actingUserGuid = Guid.NewGuid();
            _currentUser.UserId.Returns(actingUserGuid.ToString());
            _fiscalClock.VenezuelaNow.Returns(new DateTime(2026, 8, 10));

            var result = await CreateHandler().Handle(new DeleteUserCommand(_userGuid), CancellationToken.None);

            Assert.False(result.IsError);
            Assert.False(user.IsActive);
            // GetUserIdOrNull() debe resolver el UserId de QUIEN EJECUTA la acción (el admin),
            // no el del usuario desactivado — son entidades distintas.
            await _eventLogRepository.Received(1).AddAsync(
                Arg.Is<EventLog>(e => e.Type == EventLogType.UserDeactivated && e.UserId == actingUserGuid && e.EntityId == user.Id.Value),
                Arg.Any<CancellationToken>());
        }
    }
}
