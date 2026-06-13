using Identity.Application.Common.Interfaces;
using Identity.Domain;
using Identity.Domain.DomainErrors;
using Identity.Domain.Repositories;
using Identity.Domain.VO;

namespace Identity.Application.Roles.Commands.UpdateRole
{
    public sealed class UpdateRoleCommandHandler
        : IRequestHandler<UpdateRoleCommand, ErrorOr<Updated>>
    {
        private readonly IRoleRepository _roleRepository;
        private readonly ISystemActionRepository _systemActionRepository;
        private readonly IIdentityUnitOfWork _unitOfWork;
        private readonly IRoleNameUniquenessChecker _roleNameUniquenessChecker;

        public UpdateRoleCommandHandler(
            IRoleRepository roleRepository,
            ISystemActionRepository systemActionRepository,
            IIdentityUnitOfWork unitOfWork,
            IRoleNameUniquenessChecker roleNameUniquenessChecker)
        {
            _roleRepository = roleRepository;
            _systemActionRepository = systemActionRepository;
            _unitOfWork = unitOfWork;
            _roleNameUniquenessChecker = roleNameUniquenessChecker;
        }

        public async Task<ErrorOr<Updated>> Handle(
            UpdateRoleCommand command,
            CancellationToken cancellationToken)
        {
            var role = await _roleRepository.GetByIdAsync(RoleId.Create(command.Id), cancellationToken);
            if (role is null)
                return RoleErrors.RoleNotFound;

            if (!string.Equals(role.Name, command.Name, StringComparison.OrdinalIgnoreCase))
            {
                if (!await _roleNameUniquenessChecker.IsRoleNameUniqueAsync(command.Name, cancellationToken))
                    return RoleErrors.NameAlreadyInUse;
            }

            role.ChangeName(command.Name);

            if (command.ActionIds is not null)
            {
                if (command.ActionIds.Count > 0)
                {
                    var existingIds = await _systemActionRepository.GetExistingIdsAsync(command.ActionIds, cancellationToken);
                    if (existingIds.Count != command.ActionIds.Count)
                        return RoleErrors.InvalidAction;

                    role.SyncActions(existingIds);
                }
                else
                {
                    role.SyncActions(Enumerable.Empty<ActionsId>());
                }
            }

            await _roleRepository.UpdateAsync(role, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Updated;
        }
    }
}
