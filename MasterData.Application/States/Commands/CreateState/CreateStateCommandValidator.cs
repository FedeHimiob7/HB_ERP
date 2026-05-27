using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterData.Application.States.Commands.CreateState
{
    public class CreateStateCommandValidator : AbstractValidator<CreateStateCommand>
    {
        public CreateStateCommandValidator()
        {
            RuleFor(x => x.CountryId)
                .NotEmpty().WithMessage("El identificador del país es obligatorio.");

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("El código del estado/provincia es obligatorio.")
                .MaximumLength(10).WithMessage("El código no puede exceder los 10 caracteres.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre del estado es obligatorio.")
                .MaximumLength(100).WithMessage("El nombre no puede exceder los 100 caracteres.");
        }
    }
}
