using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterData.Application.Currencies.Commands.UpdateCurrency
{
    public class UpdateCurrencyCommandValidator : AbstractValidator<UpdateCurrencyCommand>
    {
        public UpdateCurrencyCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("El ID de la moneda es obligatorio.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre de la moneda es obligatorio.")
                .MaximumLength(100).WithMessage("El nombre no puede exceder los 100 caracteres.");

            RuleFor(x => x.Symbol)
                .NotEmpty().WithMessage("El símbolo es obligatorio.")
                .MaximumLength(10).WithMessage("El símbolo no puede exceder los 10 caracteres.");
        }
    }
}
