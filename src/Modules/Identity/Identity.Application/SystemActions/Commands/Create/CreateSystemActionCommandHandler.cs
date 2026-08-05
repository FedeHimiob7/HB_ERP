using HB_ERP.SharedKernel.Domain.Primitives;
using Identity.Application.Common.Extensions;
using Identity.Application.Common.Interfaces;
using Identity.Application.Users.Commands.RegisterUser;
using Identity.Domain;
using Identity.Domain.Common;
using Identity.Domain.DomainErrors;
using Identity.Domain.Entities;
using Identity.Domain.Enums;
using Identity.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.SystemActions.Commands.Create
{
    public sealed class CreateSystemActionCommandHandler : IRequestHandler<CreateSystemActionCommand, ErrorOr<Guid>>
{
    private readonly ISystemActionRepository _systemActionRepository;
    private readonly IIdentityUnitOfWork _unitOfWork;
    private readonly IEventLogRepository _eventLogRepository;
    private readonly IFiscalClock _fiscalClock;
    private readonly ICurrentUserProvider _currentUser;

    public CreateSystemActionCommandHandler(
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

    public async Task<ErrorOr<Guid>> Handle(CreateSystemActionCommand request, CancellationToken cancellationToken)
    {
        bool isUnique = await _systemActionRepository.IsNameUniqueAsync(request.Name, cancellationToken);

        if (!isUnique)
        {
            return SystemActionErrors.DuplicateName;
        }

        var systemAction = SystemAction.Create(request.Name, request.Description);

        await _systemActionRepository.AddAsync(systemAction, cancellationToken);

        var eventLog = EventLog.Create(
            EventLogType.SystemActionCreated,
            _fiscalClock.VenezuelaNow,
            $"Acción de sistema '{request.Name}' creada",
            _currentUser.GetUserIdOrNull(),
            entityType: nameof(SystemAction),
            entityId: systemAction.Id.Value);
        await _eventLogRepository.AddAsync(eventLog, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return systemAction.Id.Value;
    }
}
}
