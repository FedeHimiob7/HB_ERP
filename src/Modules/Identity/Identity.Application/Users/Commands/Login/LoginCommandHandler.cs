using Identity.Application.Common.Interfaces;
using Identity.Domain;
using Identity.Domain.DomainErrors;
using Identity.Domain.Interface;
using Identity.Domain.VO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.Users.Commands.Login
{
    public sealed class LoginCommandHandler(
     IUserRepository userRepository,
     IPasswordHasher passwordHasher,
     IJwtTokenService jwtTokenService)
     : IRequestHandler<LoginCommand, ErrorOr<string>>
    {
        public async Task<ErrorOr<string>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            
            var emailResult = Email.Create(request.Email);
            if (emailResult.IsError)
            {
                return UserErrors.InvalidCredentials;
            }
            var user = await userRepository.GetByEmailAsync(emailResult.Value.ToString());

            if (user is null)
            {
                return UserErrors.InvalidCredentials;
            }

            bool isPasswordValid = passwordHasher.Verify(request.Password, user.PasswordHash.Value);

            if (!isPasswordValid)
            {
                return UserErrors.InvalidCredentials;
            }
            var token = jwtTokenService.GenerateToken(user);

            return token;
        }
    }
}
