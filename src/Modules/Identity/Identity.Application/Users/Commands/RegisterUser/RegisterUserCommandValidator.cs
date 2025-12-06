using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.Users.Commands.RegisterUser
{
    public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
    {
        public RegisterUserCommandValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty()
                .MaximumLength(50);

            RuleFor(x => x.LastName)
                .NotEmpty()
                .MaximumLength(50);

            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress();

            RuleFor(x => x.Password)
                .NotEmpty()
                .MinimumLength(8)            
                .Matches(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[!@#$%^&*()_\-+=\[{\]};:<>|./?]).+$") //esto para q contenga al menos una letra, un número y un caracter especial
                .WithMessage("La contraseña debe contener al menos una letra, un número y un caracter especial.");
        }
    }
}
