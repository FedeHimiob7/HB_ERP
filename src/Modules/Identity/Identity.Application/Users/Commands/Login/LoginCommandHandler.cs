using HB_ERP.SharedKernel.Domain.Primitives;
using Identity.Application.Common.Interfaces;
using Identity.Domain;
using Identity.Domain.DomainErrors;
using Identity.Domain.Entities;
using Identity.Domain.Enums;
using Identity.Domain.Interface;
using Identity.Domain.Repositories;
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
     IJwtTokenService jwtTokenService,
     IEventLogRepository eventLogRepository,
     IIdentityUnitOfWork unitOfWork,
     IFiscalClock fiscalClock)
     : IRequestHandler<LoginCommand, ErrorOr<string>>
    {
        public async Task<ErrorOr<string>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {

            var emailResult = Email.Create(request.Email);
            if (emailResult.IsError)
            {
                await LogAttemptAsync(EventLogType.LoginFailed, null, request.Email, cancellationToken);
                return UserErrors.InvalidCredentials;
            }
            var user = await userRepository.GetByEmailAsync(emailResult.Value.ToString());

            if (user is null)
            {
                await LogAttemptAsync(EventLogType.LoginFailed, null, request.Email, cancellationToken);
                return UserErrors.InvalidCredentials;
            }

            bool isPasswordValid = passwordHasher.Verify(request.Password, user.PasswordHash.Value);

            if (!isPasswordValid)
            {
                await LogAttemptAsync(EventLogType.LoginFailed, user.Id.Value, request.Email, cancellationToken);
                return UserErrors.InvalidCredentials;
            }
            var token = await jwtTokenService.GenerateTokenAsync(user);

            await LogAttemptAsync(EventLogType.LoginSucceeded, user.Id.Value, request.Email, cancellationToken);

            return token;
        }

        private async Task LogAttemptAsync(EventLogType type, Guid? userId, string attemptedEmail, CancellationToken cancellationToken)
        {
            var description = type == EventLogType.LoginSucceeded
                ? $"Login exitoso para '{attemptedEmail}'"
                : $"Intento de login fallido para '{attemptedEmail}'";

            var eventLog = EventLog.Create(type, fiscalClock.VenezuelaNow, description, userId, attemptedEmail);
            await eventLogRepository.AddAsync(eventLog, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
