using FluentValidation;

namespace MasterData.Application.FiscalTerminals.Commands.UpdateFiscalTerminal
{
    public class UpdateFiscalTerminalCommandValidator : AbstractValidator<UpdateFiscalTerminalCommand>
    {
        public UpdateFiscalTerminalCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre del punto de emisión es obligatorio.")
                .MaximumLength(100).WithMessage("El nombre no puede exceder los 100 caracteres.");

            RuleFor(x => x.EmissionMethod).IsInEnum();
        }
    }
}
