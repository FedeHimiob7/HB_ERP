using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.Users.Commands.Login
{
    internal class LoginCommandValidator : AbstractValidator<LoginCommand>
    {
        public LoginCommandValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("El correo electrónico es obligatorio.")
                .EmailAddress().WithMessage("El formato del correo electrónico no es válido.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("La contraseña es obligatoria.")
                .MinimumLength(6).WithMessage("La contraseña debe tener al menos 6 caracteres.");
        }
    }
}
