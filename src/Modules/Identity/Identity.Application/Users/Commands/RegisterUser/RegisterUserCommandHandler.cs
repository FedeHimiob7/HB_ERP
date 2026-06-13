using Identity.Application.Common.Interfaces;
using Identity.Domain;
using Identity.Domain.DomainErrors;
using Identity.Domain.Interface;
using Identity.Domain.Repositories;
using Identity.Domain.VO;

namespace Identity.Application.Users.Commands.RegisterUser
{
    public sealed class RegisterUserCommandHandler
    : IRequestHandler<RegisterUserCommand, ErrorOr<Guid>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IIdentityUnitOfWork _unitOfWork;
        private readonly IUserEmailUniquenessChecker _emailChecker;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IRoleRepository _roleRepository;
        private readonly IPslExistenceChecker _pslChecker;

        public RegisterUserCommandHandler(
            IUserRepository userRepository,
            IIdentityUnitOfWork unitOfWork,
            IUserEmailUniquenessChecker userEmailUniquenessChecker,
            IPasswordHasher passwordHasher,
            IRoleRepository roleRepository,
            IPslExistenceChecker pslChecker)
        {
            _userRepository = userRepository
                ?? throw new ArgumentNullException(nameof(userRepository));

            _unitOfWork = unitOfWork
                ?? throw new ArgumentNullException(nameof(unitOfWork));

            _emailChecker = userEmailUniquenessChecker
                ?? throw new ArgumentNullException(nameof(userEmailUniquenessChecker));

            _passwordHasher = passwordHasher
                ?? throw new ArgumentNullException(nameof(passwordHasher));

            _roleRepository = roleRepository
                ?? throw new ArgumentNullException(nameof(roleRepository));

            _pslChecker = pslChecker
                ?? throw new ArgumentNullException(nameof(pslChecker));
        }

        public async Task<ErrorOr<Guid>> Handle(
            RegisterUserCommand command,
            CancellationToken cancellationToken)
        {
            var userEmail = Email.Create(command.Email);
            if (userEmail.IsError)
                return userEmail.Errors;

            var hash = _passwordHasher.Hash(command.Password);
            var passwordHash = PasswordHash.Create(hash);

            if (!await _emailChecker.IsEmailUniqueAsync(userEmail.Value, cancellationToken))
                return UserErrors.EmailAlreadyInUse;


            var user = User.Register(command.FirstName, command.LastName, userEmail.Value, passwordHash);

            if (command.RoleIds is { Count: > 0 })
            {
                var existingRoleIds = await _roleRepository.GetExistingIdsAsync(command.RoleIds, cancellationToken);
                if (existingRoleIds.Count != command.RoleIds.Count)
                    return RoleErrors.RoleNotFound;

                user.SyncRoles(command.RoleIds.Select(RoleId.Create));
            }

            if (command.PslIds is { Count: > 0 })
            {
                foreach (var pslId in command.PslIds)
                {
                    if (!await _pslChecker.ExistsAsync(pslId, cancellationToken))
                        return UserErrors.InvalidPsl;
                }

                user.SyncPsls(command.PslIds.Select(PslId.Create));
            }

            await _userRepository.AddAsync(user);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return user.Id.Value;
        }
    }
}
