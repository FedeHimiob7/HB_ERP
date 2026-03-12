using Identity.Application.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.Users.Queries.GetUserById
{   
        public record GetUserByIdQuery(Guid UserId) : IRequest<ErrorOr<UserResponse>>;    
}
