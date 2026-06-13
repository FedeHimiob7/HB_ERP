using Identity.Application.Common.Interfaces;
using Identity.Domain.DomainErrors;
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

        public DeleteSystemActionCommandHandler(
            ISystemActionRepository systemActionRepository,
            IRoleRepository roleRepository,
            IIdentityUnitOfWork unitOfWork)
        {
            _systemActionRepository = systemActionRepository;
            _roleRepository = roleRepository;
            _unitOfWork = unitOfWork;
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

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Deleted;
        }
    }
}
