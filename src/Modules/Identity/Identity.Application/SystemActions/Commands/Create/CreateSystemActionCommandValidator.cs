using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.SystemActions.Commands.Create
{
    public sealed class CreateSystemActionCommandValidator : AbstractValidator<CreateSystemActionCommand>
    {
        public CreateSystemActionCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre de la acción es obligatorio.")
                .MinimumLength(3).WithMessage("El nombre debe tener al menos 3 caracteres.")
                .MaximumLength(100).WithMessage("El nombre no puede exceder los 100 caracteres.")
                .Matches(@"^[a-zA-Z0-9\-\.]+$").WithMessage("El nombre contiene caracteres inválidos.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("La descripción es obligatoria.");
        }
    }
}
