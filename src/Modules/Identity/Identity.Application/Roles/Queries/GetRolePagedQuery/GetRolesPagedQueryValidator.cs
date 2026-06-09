using Identity.Application.Users.Queries.GetUsersPaged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.Roles.Queries.GetRolePagedQuery
{
    internal class GetRolesPagedQueryValidator : AbstractValidator<GetRolesPagedQuery>
    {
        public GetRolesPagedQueryValidator()
        {
            RuleFor(x => x.PageNumber)
                .GreaterThanOrEqualTo(1)
                .WithMessage("El número de página debe ser mayor o igual a 1.");

            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 100)
                .WithMessage("El tamaño de página debe estar entre 1 y 100 registros.");
        }
    }
}
