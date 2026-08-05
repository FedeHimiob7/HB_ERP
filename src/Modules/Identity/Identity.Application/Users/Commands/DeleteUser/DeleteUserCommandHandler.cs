using HB_ERP.SharedKernel.Domain.Primitives;
using Identity.Application.Common.Extensions;
using Identity.Application.Common.Interfaces;
using Identity.Domain;
using Identity.Domain.DomainErrors;
using Identity.Domain.Entities;
using Identity.Domain.Enums;
using Identity.Domain.Repositories;
using Identity.Domain.VO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.Users.Commands.DeleteUser
{
    internal sealed class DeleteUserCommandHandler(
    IUserRepository _userRepository,
    IIdentityUnitOfWork _unitOfWork,
    IEventLogRepository _eventLogRepository,
    IFiscalClock _fiscalClock,
    ICurrentUserProvider _currentUser)
    : IRequestHandler<DeleteUserCommand, ErrorOr<Success>>
    {
        public async Task<ErrorOr<Success>> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(UserId.Create(request.UserId));

            if (user is null)
            {
                return UserErrors.NotFound;
            }

            user.Deactivate();

            await _userRepository.UpdateAsync(user);

            var eventLog = EventLog.Create(
                EventLogType.UserDeactivated,
                _fiscalClock.VenezuelaNow,
                $"Usuario '{user.Email.Value}' desactivado",
                _currentUser.GetUserIdOrNull(),
                entityType: nameof(User),
                entityId: user.Id.Value);
            await _eventLogRepository.AddAsync(eventLog, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success;
        }
    }
}
