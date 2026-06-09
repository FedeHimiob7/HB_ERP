using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterData.Application.ProductServiceLines.Commands.UpdatePSL
{
    public class UpdateProductServiceLineCommandValidator : AbstractValidator<UpdateProductServiceLineCommand>
    {
        public UpdateProductServiceLineCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("El identificador de la línea de servicio es obligatorio.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre de la línea de servicio es obligatorio.")
                .MaximumLength(100).WithMessage("El nombre no puede exceder los 100 caracteres.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("La descripción es obligatoria.")
                .MaximumLength(250).WithMessage("La descripción no puede exceder los 250 caracteres.");
        }
    }
}
