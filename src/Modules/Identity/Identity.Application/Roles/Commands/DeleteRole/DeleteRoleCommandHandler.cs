using HB_ERP.SharedKernel.Domain.Primitives;
using Identity.Application.Common.Extensions;
using Identity.Application.Common.Interfaces;
using Identity.Domain;
using Identity.Domain.DomainErrors;
using Identity.Domain.Entities;
using Identity.Domain.Enums;
using Identity.Domain.Repositories;
using Identity.Domain.VO;

namespace Identity.Application.Roles.Commands.DeleteRole
{
    public sealed class DeleteRoleCommandHandler
        : IRequestHandler<DeleteRoleCommand, ErrorOr<Deleted>>
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IUserRepository _userRepository;
        private readonly IIdentityUnitOfWork _unitOfWork;
        private readonly IEventLogRepository _eventLogRepository;
        private readonly IFiscalClock _fiscalClock;
        private readonly ICurrentUserProvider _currentUser;

        public DeleteRoleCommandHandler(
            IRoleRepository roleRepository,
            IUserRepository userRepository,
            IIdentityUnitOfWork unitOfWork,
            IEventLogRepository eventLogRepository,
            IFiscalClock fiscalClock,
            ICurrentUserProvider currentUser)
        {
            _roleRepository = roleRepository;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _eventLogRepository = eventLogRepository;
            _fiscalClock = fiscalClock;
            _currentUser = currentUser;
        }

        public async Task<ErrorOr<Deleted>> Handle(
            DeleteRoleCommand command,
            CancellationToken cancellationToken)
        {
            var role = await _roleRepository.GetByIdAsync(RoleId.Create(command.Id), cancellationToken);
            if (role is null)
                return RoleErrors.RoleNotFound;

            role.Deactivate();
            await _roleRepository.UpdateAsync(role, cancellationToken);

            var affectedUsers = await _userRepository.GetUsersByRoleIdAsync(command.Id, cancellationToken);
            foreach (var user in affectedUsers)
            {
                user.RemoveRole(new RoleId(command.Id));
                await _userRepository.UpdateAsync(user);
            }

            var eventLog = EventLog.Create(
                EventLogType.RoleDeactivated,
                _fiscalClock.VenezuelaNow,
                $"Rol '{role.Name}' desactivado; removido de {affectedUsers.Count} usuario(s)",
                _currentUser.GetUserIdOrNull(),
                entityType: nameof(Role),
                entityId: role.Id.Value);
            await _eventLogRepository.AddAsync(eventLog, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Deleted;
        }
    }
}
