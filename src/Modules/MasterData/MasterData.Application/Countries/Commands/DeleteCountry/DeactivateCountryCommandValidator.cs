using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterData.Application.Countries.Commands.DeleteCountry
{
    public class DeactivateCountryCommandValidator : AbstractValidator<DeactivateCountryCommand>
    {
        public DeactivateCountryCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("El identificador del país es obligatorio.");
        }
    }
}
