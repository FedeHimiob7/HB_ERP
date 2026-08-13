using HB_ERP.SharedKernel.Domain.Primitives;
using Identity.Application.Common.Interfaces;
using Identity.Application.Users.Commands.ResetUserPassword;
using Identity.Domain;
using Identity.Domain.Entities;
using Identity.Domain.Enums;
using Identity.Domain.Interface;
using Identity.Domain.VO;
using NSubstitute;

namespace Application.Tests
{
    public sealed class ResetUserPasswordCommandHandlerTests
    {
        private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
        private readonly IIdentityUnitOfWork _unitOfWork = Substitute.For<IIdentityUnitOfWork>();
        private readonly IPasswordHasher _passwordHasher = Substitute.For<IPasswordHasher>();
        private readonly Identity.Domain.Repositories.IEventLogRepository _eventLogRepository = Substitute.For<Identity.Domain.Repositories.IEventLogRepository>();
        private readonly IFiscalClock _fiscalClock = Substitute.For<IFiscalClock>();
        private readonly ICurrentUserProvider _currentUser = Substitute.For<ICurrentUserProvider>();
        private readonly Guid _userGuid = Guid.NewGuid();

        private ResetUserPasswordCommandHandler CreateHandler()
            => new(_userRepository, _unitOfWork, _passwordHasher, _eventLogRepository, _fiscalClock, _currentUser);

        [Fact]
        public async Task Handle_WhenValid_WritesUserPasswordResetEventLog()
        {
            var user = User.Register("Juan", "Perez", Email.Create("juan@ejemplo.com").Value, PasswordHash.Create("hash-viejo"));
            _userRepository.GetByIdAsync(Arg.Any<UserId>()).Returns(user);
            _passwordHasher.Hash(Arg.Any<string>()).Returns("hash-nuevo");
            _fiscalClock.VenezuelaNow.Returns(new DateTime(2026, 8, 10));

            var result = await CreateHandler().Handle(new ResetUserPasswordCommand(_userGuid, "clave-nueva"), CancellationToken.None);

            Assert.False(result.IsError);
            Assert.Equal("hash-nuevo", user.PasswordHash!.Value);
            await _eventLogRepository.Received(1).AddAsync(
                Arg.Is<EventLog>(e => e.Type == EventLogType.UserPasswordReset && e.EntityId == user.Id.Value),
                Arg.Any<CancellationToken>());
        }
    }
}
