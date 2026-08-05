using HB_ERP.SharedKernel.Domain.Primitives;
using Identity.Application.Common.Extensions;
using Identity.Application.Common.Interfaces;
using Identity.Domain.DomainErrors;
using Identity.Domain.Entities;
using Identity.Domain.Enums;
using Identity.Domain.Repositories;
using Identity.Domain.VO;

namespace Identity.Application.SystemActions.Commands.DeleteSystemAction
{
    public sealed class DeleteSystemActionCommandHandler
        : IRequestHandler<DeleteSystemActionCommand, ErrorOr<Deleted>>
    {
        private readonly ISystemActionRepository _systemActionRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IIdentityUnitOfWork _unitOfWork;
        private readonly IEventLogRepository _eventLogRepository;
        private readonly IFiscalClock _fiscalClock;
        private readonly ICurrentUserProvider _currentUser;

        public DeleteSystemActionCommandHandler(
            ISystemActionRepository systemActionRepository,
            IRoleRepository roleRepository,
            IIdentityUnitOfWork unitOfWork,
            IEventLogRepository eventLogRepository,
            IFiscalClock fiscalClock,
            ICurrentUserProvider currentUser)
        {
            _systemActionRepository = systemActionRepository;
            _roleRepository = roleRepository;
            _unitOfWork = unitOfWork;
            _eventLogRepository = eventLogRepository;
            _fiscalClock = fiscalClock;
            _currentUser = currentUser;
        }

        public async Task<ErrorOr<Deleted>> Handle(
            DeleteSystemActionCommand command,
            CancellationToken cancellationToken)
        {
            var action = await _systemActionRepository.GetByIdAsync(new ActionsId(command.Id));
            if (action is null)
                return SystemActionErrors.NotFound;

            action.Deactivate();
            await _systemActionRepository.UpdateAsync(action);

            // Limpiamos la acción de todos los roles que la tenían asignada.
            // Todo se persiste en el mismo SaveChangesAsync (una sola transacción).
            var affectedRoles = await _roleRepository.GetRolesByActionIdAsync(command.Id, cancellationToken);
            foreach (var role in affectedRoles)
            {
                role.RevokeAction(new ActionsId(command.Id));
                await _roleRepository.UpdateAsync(role, cancellationToken);
            }

            var eventLog = EventLog.Create(
                EventLogType.SystemActionDeactivated,
                _fiscalClock.VenezuelaNow,
                $"Acción de sistema '{action.Name.Value}' desactivada; removida de {affectedRoles.Count} rol(es)",
                _currentUser.GetUserIdOrNull(),
                entityType: nameof(SystemAction),
                entityId: action.Id.Value);
            await _eventLogRepository.AddAsync(eventLog, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Deleted;
        }
    }
}
