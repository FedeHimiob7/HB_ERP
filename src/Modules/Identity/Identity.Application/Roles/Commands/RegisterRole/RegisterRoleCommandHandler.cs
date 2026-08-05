
using HB_ERP.SharedKernel.Domain.Primitives;
using Identity.Application.Common.Extensions;
using Identity.Application.Common.Interfaces;
using Identity.Domain;
using Identity.Domain.DomainErrors;
using Identity.Domain.Entities;
using Identity.Domain.Enums;
using Identity.Domain.Repositories;
using Identity.Domain.VO;

namespace Identity.Application.Roles.Commands.RegisterRole
{
    public sealed class RegisterRoleCommandHandler
        : IRequestHandler<RegisterRoleCommand, ErrorOr<Guid>>
    {
        private readonly IRoleRepository _roleRepository;
        private readonly ISystemActionRepository _systemActionRepository;
        private readonly IIdentityUnitOfWork _unitOfWork;
        private readonly IRoleNameUniquenessChecker _roleNameUniquenessChecker;
        private readonly IEventLogRepository _eventLogRepository;
        private readonly IFiscalClock _fiscalClock;
        private readonly ICurrentUserProvider _currentUser;

        public RegisterRoleCommandHandler(
            IRoleRepository roleRepository,
            ISystemActionRepository systemActionRepository,
            IIdentityUnitOfWork unitOfWork,
            IRoleNameUniquenessChecker roleNameUniquenessChecker,
            IEventLogRepository eventLogRepository,
            IFiscalClock fiscalClock,
            ICurrentUserProvider currentUser)
        {
            _roleRepository = roleRepository;
            _systemActionRepository = systemActionRepository;
            _unitOfWork = unitOfWork;
            _roleNameUniquenessChecker = roleNameUniquenessChecker;
            _eventLogRepository = eventLogRepository;
            _fiscalClock = fiscalClock;
            _currentUser = currentUser;
        }

        public async Task<ErrorOr<Guid>> Handle(
            RegisterRoleCommand command,
            CancellationToken cancellationToken)
        {
            if (!await _roleNameUniquenessChecker.IsRoleNameUniqueAsync(command.Name, cancellationToken))
                return RoleErrors.NameAlreadyInUse;

            var role = Role.Create(command.Name);

            if (command.ActionIds is { Count: > 0 })
            {
                var existingIds = await _systemActionRepository.GetExistingIdsAsync(command.ActionIds, cancellationToken);
                if (existingIds.Count != command.ActionIds.Count)
                    return RoleErrors.InvalidAction;

                role.SyncActions(existingIds);
            }

            await _roleRepository.AddAsync(role);

            var eventLog = EventLog.Create(
                EventLogType.RoleCreated,
                _fiscalClock.VenezuelaNow,
                $"Rol '{role.Name}' creado",
                _currentUser.GetUserIdOrNull(),
                entityType: nameof(Role),
                entityId: role.Id.Value);
            await _eventLogRepository.AddAsync(eventLog, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return role.Id.Value;
        }
    }
}
