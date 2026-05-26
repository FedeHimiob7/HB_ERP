using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterData.Application.ProductServiceLines.Commands.DesactivatePSL
{
    public class DeactivateProductServiceLineCommandValidator : AbstractValidator<DeactivateProductServiceLineCommand>
    {
        public DeactivateProductServiceLineCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("El identificador de la línea de servicio es obligatorio.");
        }
    }
}
