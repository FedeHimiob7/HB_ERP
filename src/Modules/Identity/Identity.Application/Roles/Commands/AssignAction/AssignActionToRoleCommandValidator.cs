using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.Roles.Commands.AssignAction
{
    public sealed class AssignActionToRoleCommandValidator : AbstractValidator<AssignActionToRoleCommand>
    {
        public AssignActionToRoleCommandValidator()
        {
            RuleFor(x => x.RoleId)
                .NotEmpty().WithMessage("El identificador del rol es obligatorio.");

            RuleFor(x => x.ActionId)
                .NotEmpty().WithMessage("El identificador de la acción de sistema es obligatorio.");
        }
    }
}
