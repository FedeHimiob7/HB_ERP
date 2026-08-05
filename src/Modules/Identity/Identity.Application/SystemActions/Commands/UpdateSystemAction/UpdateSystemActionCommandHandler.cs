using HB_ERP.SharedKernel.Domain.Primitives;
using Identity.Application.Common.Extensions;
using Identity.Application.Common.Interfaces;
using Identity.Domain.DomainErrors;
using Identity.Domain.Entities;
using Identity.Domain.Enums;
using Identity.Domain.Repositories;
using Identity.Domain.VO;

namespace Identity.Application.SystemActions.Commands.UpdateSystemAction
{
    public sealed class UpdateSystemActionCommandHandler
        : IRequestHandler<UpdateSystemActionCommand, ErrorOr<Updated>>
    {
        private readonly ISystemActionRepository _systemActionRepository;
        private readonly IIdentityUnitOfWork _unitOfWork;
        private readonly IEventLogRepository _eventLogRepository;
        private readonly IFiscalClock _fiscalClock;
        private readonly ICurrentUserProvider _currentUser;

        public UpdateSystemActionCommandHandler(
            ISystemActionRepository systemActionRepository,
            IIdentityUnitOfWork unitOfWork,
            IEventLogRepository eventLogRepository,
            IFiscalClock fiscalClock,
            ICurrentUserProvider currentUser)
        {
            _systemActionRepository = systemActionRepository;
            _unitOfWork = unitOfWork;
            _eventLogRepository = eventLogRepository;
            _fiscalClock = fiscalClock;
            _currentUser = currentUser;
        }

        public async Task<ErrorOr<Updated>> Handle(
            UpdateSystemActionCommand command,
            CancellationToken cancellationToken)
        {
            var action = await _systemActionRepository.GetByIdAsync(new ActionsId(command.Id));
            if (action is null)
                return SystemActionErrors.NotFound;

            if (!string.Equals(action.Name.Value, command.Name, StringComparison.OrdinalIgnoreCase))
            {
                if (!await _systemActionRepository.IsNameUniqueAsync(command.Name, cancellationToken))
                    return SystemActionErrors.DuplicateName;
            }

            action.UpdateDetails(ActionName.Create(command.Name), command.Description);

            await _systemActionRepository.UpdateAsync(action);

            var eventLog = EventLog.Create(
                EventLogType.SystemActionUpdated,
                _fiscalClock.VenezuelaNow,
                $"Acción de sistema '{action.Name.Value}' actualizada",
                _currentUser.GetUserIdOrNull(),
                entityType: nameof(SystemAction),
                entityId: action.Id.Value);
            await _eventLogRepository.AddAsync(eventLog, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Updated;
        }
    }
}
