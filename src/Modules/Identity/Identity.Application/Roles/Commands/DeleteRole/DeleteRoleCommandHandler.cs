using Identity.Application.Common.Interfaces;
using Identity.Domain;
using Identity.Domain.DomainErrors;
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

        public DeleteRoleCommandHandler(
            IRoleRepository roleRepository,
            IUserRepository userRepository,
            IIdentityUnitOfWork unitOfWork)
        {
            _roleRepository = roleRepository;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
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

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Deleted;
        }
    }
}
