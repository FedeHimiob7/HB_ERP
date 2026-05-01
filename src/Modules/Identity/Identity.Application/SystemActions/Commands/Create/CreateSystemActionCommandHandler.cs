using Identity.Application.Common.Interfaces;
using Identity.Application.Users.Commands.RegisterUser;
using Identity.Domain;
using Identity.Domain.Common;
using Identity.Domain.DomainErrors;
using Identity.Domain.Entities;
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
    private readonly IUnitOfWork _unitOfWork;

    public CreateSystemActionCommandHandler(
        ISystemActionRepository systemActionRepository,
        IUnitOfWork unitOfWork)
    {
        _systemActionRepository = systemActionRepository;
        _unitOfWork = unitOfWork;
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
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return systemAction.Id.Value;
    }
}
}
