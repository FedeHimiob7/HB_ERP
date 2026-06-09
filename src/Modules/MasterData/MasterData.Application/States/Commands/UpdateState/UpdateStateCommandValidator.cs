using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterData.Application.States.Commands.UpdateState
{
    public class UpdateStateCommandValidator : AbstractValidator<UpdateStateCommand>
    {
        public UpdateStateCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("El identificador del estado es obligatorio.");
            RuleFor(x => x.CountryId).NotEmpty().WithMessage("El identificador del país es obligatorio.");
            RuleFor(x => x.Code).NotEmpty().WithMessage("El código del estado es obligatorio.");
            RuleFor(x => x.Name).NotEmpty().WithMessage("El nombre del estado es obligatorio.");
        }
    }
}
