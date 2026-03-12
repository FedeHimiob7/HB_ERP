using Identity.Application.Common.Models;
using Identity.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.Users.Queries.GetUsersPaged
{
    public sealed class GetUsersPagedQueryHandler(IUserRepository userRepository)
     : IRequestHandler<GetUsersPagedQuery, ErrorOr<PagedList<UserResponse>>>
    {
        public async Task<ErrorOr<PagedList<UserResponse>>> Handle(
            GetUsersPagedQuery request,
            CancellationToken cancellationToken)
        {
            
            var (users, totalCount) = await userRepository.GetPagedAsync(
                request.PageNumber,
                request.PageSize,
                cancellationToken);

            var userResponses = users.Select(user => new UserResponse(
                user.Id.Value,
                user.FirstName,
                user.LastName,
                user.Email.Value,
                user.IsActive,
                user.Roles.Select(r => r.Value).ToList()
            )).ToList();

            return new PagedList<UserResponse>(userResponses, totalCount, request.PageNumber, request.PageSize);
        }
    }
}
