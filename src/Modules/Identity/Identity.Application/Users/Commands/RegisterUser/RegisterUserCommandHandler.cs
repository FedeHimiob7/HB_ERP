using Identity.Application.Common.Interfaces;
using Identity.Domain;
using Identity.Domain.DomainErrors;
using Identity.Domain.Interface;
using Identity.Domain.VO;

namespace Identity.Application.Users.Commands.RegisterUser
{
    public sealed class RegisterUserCommandHandler
    : IRequestHandler<RegisterUserCommand, ErrorOr<Guid>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserEmailUniquenessChecker _emailChecker;
        private readonly IPasswordHasher _passwordHasher;

        public RegisterUserCommandHandler(
            IUserRepository userRepository,
            IUnitOfWork unitOfWork,
            IUserEmailUniquenessChecker userEmailUniquenessChecker,
            IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository
                ?? throw new ArgumentNullException(nameof(userRepository));

            _unitOfWork = unitOfWork
                ?? throw new ArgumentNullException(nameof(unitOfWork));

            _emailChecker = userEmailUniquenessChecker 
                ?? throw new ArgumentNullException(nameof(userEmailUniquenessChecker));

            _passwordHasher = passwordHasher 
                ?? throw new ArgumentNullException(nameof(passwordHasher));
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


            await _userRepository.AddAsync(user);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return user.Id.Value;
        }
    }
}
