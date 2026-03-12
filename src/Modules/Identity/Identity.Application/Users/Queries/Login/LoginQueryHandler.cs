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

namespace Identity.Application.Users.Queries.Login
{
    public sealed class LoginQueryHandler(
     IUserRepository userRepository,
     IPasswordHasher passwordHasher,
     IJwtTokenService jwtTokenService)
     : IRequestHandler<LoginQuery, ErrorOr<string>>
    {
        public async Task<ErrorOr<string>> Handle(LoginQuery request, CancellationToken cancellationToken)
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
