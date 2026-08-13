using HB_ERP.SharedKernel.Domain.Primitives;
using Identity.Application.Common.Interfaces;
using Identity.Application.Users.Commands.Login;
using Identity.Domain;
using Identity.Domain.Entities;
using Identity.Domain.Enums;
using Identity.Domain.Interface;
using Identity.Domain.Repositories;
using Identity.Domain.VO;
using NSubstitute;

namespace Application.Tests
{
    // Login es el handler más instrumentado de F0: escribe EventLog tanto en éxito como en fallo,
    // porque un intento fallido de login es en sí mismo un evento de seguridad relevante de auditar.
    public sealed class LoginCommandHandlerTests
    {
        private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
        private readonly IPasswordHasher _passwordHasher = Substitute.For<IPasswordHasher>();
        private readonly IJwtTokenService _jwtTokenService = Substitute.For<IJwtTokenService>();
        private readonly IEventLogRepository _eventLogRepository = Substitute.For<IEventLogRepository>();
        private readonly IIdentityUnitOfWork _unitOfWork = Substitute.For<IIdentityUnitOfWork>();
        private readonly IFiscalClock _fiscalClock = Substitute.For<IFiscalClock>();

        private LoginCommandHandler CreateHandler()
            => new(_userRepository, _passwordHasher, _jwtTokenService, _eventLogRepository, _unitOfWork, _fiscalClock);

        [Fact]
        public async Task Handle_WhenUserDoesNotExist_WritesLoginFailedEventLog()
        {
            _userRepository.GetByEmailAsync(Arg.Any<string>()).Returns((User?)null);
            _fiscalClock.VenezuelaNow.Returns(new DateTime(2026, 8, 10));

            var result = await CreateHandler().Handle(new LoginCommand("nadie@ejemplo.com", "clave"), CancellationToken.None);

            Assert.True(result.IsError);
            await _eventLogRepository.Received(1).AddAsync(
                Arg.Is<EventLog>(e => e.Type == EventLogType.LoginFailed && e.AttemptedEmail == "nadie@ejemplo.com" && e.UserId == null),
                Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_WhenPasswordIsInvalid_WritesLoginFailedEventLogWithUserId()
        {
            var user = User.Register("Juan", "Perez", Email.Create("juan@ejemplo.com").Value, PasswordHash.Create("hash-viejo"));
            _userRepository.GetByEmailAsync(Arg.Any<string>()).Returns(user);
            _passwordHasher.Verify(Arg.Any<string>(), Arg.Any<string>()).Returns(false);
            _fiscalClock.VenezuelaNow.Returns(new DateTime(2026, 8, 10));

            var result = await CreateHandler().Handle(new LoginCommand("juan@ejemplo.com", "clave-mala"), CancellationToken.None);

            Assert.True(result.IsError);
            // A diferencia del usuario inexistente, acá sí conocemos el UserId — se registra en el log.
            await _eventLogRepository.Received(1).AddAsync(
                Arg.Is<EventLog>(e => e.Type == EventLogType.LoginFailed && e.UserId == user.Id.Value),
                Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_WhenCredentialsAreValid_WritesLoginSucceededEventLogAndReturnsToken()
        {
            var user = User.Register("Juan", "Perez", Email.Create("juan@ejemplo.com").Value, PasswordHash.Create("hash"));
            _userRepository.GetByEmailAsync(Arg.Any<string>()).Returns(user);
            _passwordHasher.Verify(Arg.Any<string>(), Arg.Any<string>()).Returns(true);
            _jwtTokenService.GenerateTokenAsync(Arg.Any<User>()).Returns("token-jwt");
            _fiscalClock.VenezuelaNow.Returns(new DateTime(2026, 8, 10));

            var result = await CreateHandler().Handle(new LoginCommand("juan@ejemplo.com", "clave-buena"), CancellationToken.None);

            Assert.False(result.IsError);
            Assert.Equal("token-jwt", result.Value);
            await _eventLogRepository.Received(1).AddAsync(
                Arg.Is<EventLog>(e => e.Type == EventLogType.LoginSucceeded && e.UserId == user.Id.Value),
                Arg.Any<CancellationToken>());
            await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }
    }
}
