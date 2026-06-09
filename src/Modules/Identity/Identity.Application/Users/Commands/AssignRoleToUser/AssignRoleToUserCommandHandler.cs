using Identity.Application.Common.Interfaces;
using Identity.Domain;
using Identity.Domain.DomainErrors;
using Identity.Domain.Repositories;
using Identity.Domain.VO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.Users.Commands.AssignRoleToUser
{
    internal class AssignRoleToUserCommandHandler(
    IUserRepository _userRepository,
    IRoleRepository _roleRepository,
    IIdentityUnitOfWork _unitOfWork)
    : IRequestHandler<AssignRoleToUserCommand, ErrorOr<Success>>
    {

        public async Task<ErrorOr<Success>> Handle(AssignRoleToUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(UserId.Create(request.UserId));
            if (user is null)
            {
                return UserErrors.NotFound;
            }

            var existingRoleIds = await _roleRepository.GetExistingIdsAsync(request.RoleIds, cancellationToken);
            if (existingRoleIds.Count != request.RoleIds.Count)
            {
                return RoleErrors.RoleNotFound;
            }
            foreach (var roleId in request.RoleIds)
            {
                user.AssignRole(RoleId.Create(roleId));
            }
            await _userRepository.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success;
        }
    }
}
