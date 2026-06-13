using Identity.Application.Common.Interfaces;
using Identity.Domain;
using Identity.Domain.DomainErrors;
using Identity.Domain.Interface;
using Identity.Domain.VO;

namespace Identity.Application.Users.Commands.ResetUserPassword
{
    public sealed class ResetUserPasswordCommandHandler
        : IRequestHandler<ResetUserPasswordCommand, ErrorOr<Success>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IIdentityUnitOfWork _unitOfWork;
        private readonly IPasswordHasher _passwordHasher;

        public ResetUserPasswordCommandHandler(
            IUserRepository userRepository,
            IIdentityUnitOfWork unitOfWork,
            IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository
                ?? throw new ArgumentNullException(nameof(userRepository));
            _unitOfWork = unitOfWork
                ?? throw new ArgumentNullException(nameof(unitOfWork));
            _passwordHasher = passwordHasher
                ?? throw new ArgumentNullException(nameof(passwordHasher));
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
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success;
        }
    }
}
