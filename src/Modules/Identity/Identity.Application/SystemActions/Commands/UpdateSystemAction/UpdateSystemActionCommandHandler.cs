using Identity.Application.Common.Interfaces;
using Identity.Domain.DomainErrors;
using Identity.Domain.Repositories;
using Identity.Domain.VO;

namespace Identity.Application.SystemActions.Commands.UpdateSystemAction
{
    public sealed class UpdateSystemActionCommandHandler
        : IRequestHandler<UpdateSystemActionCommand, ErrorOr<Updated>>
    {
        private readonly ISystemActionRepository _systemActionRepository;
        private readonly IIdentityUnitOfWork _unitOfWork;

        public UpdateSystemActionCommandHandler(
            ISystemActionRepository systemActionRepository,
            IIdentityUnitOfWork unitOfWork)
        {
            _systemActionRepository = systemActionRepository;
            _unitOfWork = unitOfWork;
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
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Updated;
        }
    }
}
