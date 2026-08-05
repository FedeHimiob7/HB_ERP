using HB_ERP.SharedKernel.Domain.Primitives;
using Identity.Application.Common.Extensions;
using Identity.Application.Common.Interfaces;
using Identity.Domain;
using Identity.Domain.DomainErrors;
using Identity.Domain.Entities;
using Identity.Domain.Enums;
using Identity.Domain.Interface;
using Identity.Domain.Repositories;
using Identity.Domain.VO;

namespace Identity.Application.Users.Commands.ResetUserPassword
{
    public sealed class ResetUserPasswordCommandHandler
        : IRequestHandler<ResetUserPasswordCommand, ErrorOr<Success>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IIdentityUnitOfWork _unitOfWork;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IEventLogRepository _eventLogRepository;
        private readonly IFiscalClock _fiscalClock;
        private readonly ICurrentUserProvider _currentUser;

        public ResetUserPasswordCommandHandler(
            IUserRepository userRepository,
            IIdentityUnitOfWork unitOfWork,
            IPasswordHasher passwordHasher,
            IEventLogRepository eventLogRepository,
            IFiscalClock fiscalClock,
            ICurrentUserProvider currentUser)
        {
            _userRepository = userRepository
                ?? throw new ArgumentNullException(nameof(userRepository));
            _unitOfWork = unitOfWork
                ?? throw new ArgumentNullException(nameof(unitOfWork));
            _passwordHasher = passwordHasher
                ?? throw new ArgumentNullException(nameof(passwordHasher));
            _eventLogRepository = eventLogRepository
                ?? throw new ArgumentNullException(nameof(eventLogRepository));
            _fiscalClock = fiscalClock
                ?? throw new ArgumentNullException(nameof(fiscalClock));
            _currentUser = currentUser
                ?? throw new ArgumentNullException(nameof(currentUser));
        }

        public async Task<ErrorOr<Success>> Handle(
            ResetUserPasswordCommand command,
            CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(UserId.Create(command.UserId));
            if (user is null)
                return UserErrors.NotFound;

            var newHash = PasswordHash.Create(_passwordHasher.Hash(command.NewPassword));
            user.ChangePassword(newHash);

            await _userRepository.UpdateAsync(user);

            var eventLog = EventLog.Create(
                EventLogType.UserPasswordReset,
                _fiscalClock.VenezuelaNow,
                $"Contraseña reiniciada para el usuario '{user.Email.Value}'",
                _currentUser.GetUserIdOrNull(),
                entityType: nameof(User),
                entityId: user.Id.Value);
            await _eventLogRepository.AddAsync(eventLog, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success;
        }
    }
}
