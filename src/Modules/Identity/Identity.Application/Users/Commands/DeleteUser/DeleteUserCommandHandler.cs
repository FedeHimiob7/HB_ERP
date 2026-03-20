using Identity.Application.Common.Interfaces;
using Identity.Domain;
using Identity.Domain.DomainErrors;
using Identity.Domain.VO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.Users.Commands.DeleteUser
{
    internal sealed class DeleteUserCommandHandler(
    IUserRepository _userRepository,
    IUnitOfWork _unitOfWork)
    : IRequestHandler<DeleteUserCommand, ErrorOr<Success>>
    {
        public async Task<ErrorOr<Success>> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {            
            var user = await _userRepository.GetByIdAsync(UserId.Create(request.UserId));

            if (user is null)
            {
                return UserErrors.NotFound;
            }            

            user.Deactivate();
           
            await _userRepository.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success;
        }
    }
}
