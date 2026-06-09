using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterData.Application.Currencies.Commands.CreateCurrencie
{
    public class CreateCurrencyCommandValidator : AbstractValidator<CreateCurrencyCommand>
    {
        public CreateCurrencyCommandValidator()
        {
            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("El código ISO es obligatorio.")
                .Length(3).WithMessage("El código ISO debe tener exactamente 3 caracteres.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre de la moneda es obligatorio.")
                .MaximumLength(100).WithMessage("El nombre no puede exceder los 100 caracteres.");

            RuleFor(x => x.Symbol)
                .NotEmpty().WithMessage("El símbolo es obligatorio.")
                .MaximumLength(10).WithMessage("El símbolo no puede exceder los 10 caracteres.");
        }
    }
}
