using FluentValidation;

namespace MasterData.Application.FiscalTerminals.Commands.CreateFiscalTerminal
{
    public class CreateFiscalTerminalCommandValidator : AbstractValidator<CreateFiscalTerminalCommand>
    {
        public CreateFiscalTerminalCommandValidator()
        {
            RuleFor(x => x.BranchId)
                .NotEmpty().WithMessage("El identificador de la sucursal es obligatorio.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre del punto de emisión es obligatorio.")
                .MaximumLength(100).WithMessage("El nombre no puede exceder los 100 caracteres.");

            RuleFor(x => x.EmissionMethod).IsInEnum();
        }
    }
}
