using Identity.Application.Users.Commands.RegisterUser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.Roles.Commands.RegisterRole
{
    internal class RegisterRoleCommandValidator : AbstractValidator<RegisterRoleCommand>
    {
        public RegisterRoleCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(50);           
        }
    }
}
