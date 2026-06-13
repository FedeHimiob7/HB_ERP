
using Identity.Application.Common.Interfaces;
using Identity.Domain;
using Identity.Domain.DomainErrors;
using Identity.Domain.Entities;
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

        public RegisterRoleCommandHandler(
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
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return role.Id.Value;
        }
    }
}
