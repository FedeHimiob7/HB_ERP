using HB_ERP.SharedKernel.Domain.Primitives;
using Identity.Application.Common.Interfaces;
using Identity.Application.Users.Commands.RegisterUser;
using Identity.Domain;
using Identity.Domain.Entities;
using Identity.Domain.Enums;
using Identity.Domain.Interface;
using Identity.Domain.Repositories;
using NSubstitute;

namespace Application.Tests
{
    public sealed class RegisterUserCommandHandlerTests
    {
        private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
        private readonly IIdentityUnitOfWork _unitOfWork = Substitute.For<IIdentityUnitOfWork>();
        private readonly IUserEmailUniquenessChecker _emailChecker = Substitute.For<IUserEmailUniquenessChecker>();
        private readonly IPasswordHasher _passwordHasher = Substitute.For<IPasswordHasher>();
        private readonly IRoleRepository _roleRepository = Substitute.For<IRoleRepository>();
        private readonly IPslExistenceChecker _pslChecker = Substitute.For<IPslExistenceChecker>();
        private readonly IEventLogRepository _eventLogRepository = Substitute.For<IEventLogRepository>();
        private readonly IFiscalClock _fiscalClock = Substitute.For<IFiscalClock>();

        private RegisterUserCommandHandler CreateHandler()
            => new(_userRepository, _unitOfWork, _emailChecker, _passwordHasher, _roleRepository, _pslChecker, _eventLogRepository, _fiscalClock);

        [Fact]
        public async Task Handle_WhenValid_WritesUserRegisteredEventLog()
        {
            _emailChecker.IsEmailUniqueAsync(Arg.Any<Identity.Domain.VO.Email>(), Arg.Any<CancellationToken>()).Returns(true);
            _passwordHasher.Hash(Arg.Any<string>()).Returns("hash");
            _fiscalClock.VenezuelaNow.Returns(new DateTime(2026, 8, 10));

            var command = new RegisterUserCommand("Juan", "Perez", "juan@ejemplo.com", "clave123");
            var result = await CreateHandler().Handle(command, CancellationToken.None);

            Assert.False(result.IsError);
            await _eventLogRepository.Received(1).AddAsync(
                Arg.Is<EventLog>(e => e.Type == EventLogType.UserRegistered && e.EntityType == nameof(User) && e.EntityId == result.Value),
                Arg.Any<CancellationToken>());
            await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }
    }
}
