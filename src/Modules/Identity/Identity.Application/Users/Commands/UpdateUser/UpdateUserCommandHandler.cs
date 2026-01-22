using Identity.Application.Common.Interfaces;
using Identity.Application.Users.Commands.RegisterUser;
using Identity.Domain;
using Identity.Domain.DomainErrors;
using Identity.Domain.VO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.Users.Commands.UpdateUser
{
    public sealed class UpdateUserCommandHandler 
        : IRequestHandler<UpdateUserCommand, ErrorOr<Guid>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserEmailUniquenessChecker _emailChecker;

        public UpdateUserCommandHandler(
            IUserRepository userRepository,
            IUnitOfWork unitOfWork,
            IUserEmailUniquenessChecker userEmailUniquenessChecker)
        {
            _userRepository = userRepository
                ?? throw new ArgumentNullException(nameof(userRepository));
            _unitOfWork = unitOfWork
                ?? throw new ArgumentNullException(nameof(unitOfWork));
            _emailChecker = userEmailUniquenessChecker
                ?? throw new ArgumentNullException(nameof(userEmailUniquenessChecker));
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

            await _userRepository.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return user.Id.Value;
        }

    }
}
