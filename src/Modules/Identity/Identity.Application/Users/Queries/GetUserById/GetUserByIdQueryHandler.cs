using Identity.Application.Common.Models;
using Identity.Domain;
using Identity.Domain.DomainErrors;
using Identity.Domain.VO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.Users.Queries.GetUserById
{
    public sealed class GetUserByIdQueryHandler(IUserRepository userRepository)
     : IRequestHandler<GetUserByIdQuery, ErrorOr<UserResponse>>
    {
        public async Task<ErrorOr<UserResponse>> Handle(
            GetUserByIdQuery request,
            CancellationToken cancellationToken)
        {
            var user = await userRepository.GetByIdAsync(UserId.Create(request.UserId));

            if (user is null)
            {
                return UserErrors.NotFound;
            }

            return new UserResponse(
                user.Id.Value,
                user.FirstName,
                user.LastName,
                user.Email.Value,
                user.IsActive,
                user.Roles.Select(r => r.Value).ToList()
                );
        }
    }
}
