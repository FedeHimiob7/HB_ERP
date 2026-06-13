using Identity.Application.Common.Interfaces;
using Identity.Domain;
using Identity.Domain.DomainErrors;
using Identity.Domain.VO;

namespace Identity.Application.Users.Commands.UpdateUser
{
    public sealed class UpdateUserCommandHandler 
        : IRequestHandler<UpdateUserCommand, ErrorOr<Guid>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IIdentityUnitOfWork _unitOfWork;
        private readonly IUserEmailUniquenessChecker _emailChecker;
        private readonly IPslExistenceChecker _pslChecker;

        public UpdateUserCommandHandler(
            IUserRepository userRepository,
            IIdentityUnitOfWork unitOfWork,
            IUserEmailUniquenessChecker userEmailUniquenessChecker,
            IPslExistenceChecker pslChecker)
        {
            _userRepository = userRepository
                ?? throw new ArgumentNullException(nameof(userRepository));
            _unitOfWork = unitOfWork
                ?? throw new ArgumentNullException(nameof(unitOfWork));
            _emailChecker = userEmailUniquenessChecker
                ?? throw new ArgumentNullException(nameof(userEmailUniquenessChecker));
            _pslChecker = pslChecker
                ?? throw new ArgumentNullException(nameof(pslChecker));
        }

        public async Task<ErrorOr<Guid>> Handle(
            UpdateUserCommand command,
            CancellationToken cancellationToken)
        {
            var userId = UserId.Create(command.UserId);
            var user = await _userRepository.GetByIdAsync(userId);
            if (user is null)
                return UserErrors.NotFound;

            user.ChangeFirstName(command.FirstName);
            user.ChangeLastName(command.LastName);


            if (command.Email is not null && command.Email != user.Email.Value)
            {
                var emailResult = Email.Create(command.Email);
                if (emailResult.IsError)
                    return emailResult.Errors;

                if (!await _emailChecker.IsEmailUniqueAsync(emailResult.Value, cancellationToken))
                    return UserErrors.EmailAlreadyInUse;

                user.ChangeEmail(emailResult.Value);
            }

            var roleIds = command.RoleIds.Select(RoleId.Create).ToList();
            user.SyncRoles(roleIds);

            if (command.PslIds is not null)
            {
                foreach (var pslId in command.PslIds)
                {
                    if (!await _pslChecker.ExistsAsync(pslId, cancellationToken))
                        return UserErrors.InvalidPsl;
                }

                user.SyncPsls(command.PslIds.Select(PslId.Create));
            }

            await _userRepository.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return user.Id.Value;
        }

    }
}
