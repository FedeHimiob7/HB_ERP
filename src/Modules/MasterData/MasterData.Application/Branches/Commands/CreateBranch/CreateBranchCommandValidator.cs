using FluentValidation;

namespace MasterData.Application.Branches.Commands.CreateBranch
{
    public class CreateBranchCommandValidator : AbstractValidator<CreateBranchCommand>
    {
        public CreateBranchCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre de la sucursal es obligatorio.")
                .MaximumLength(100).WithMessage("El nombre no puede exceder los 100 caracteres.");

            RuleFor(x => x.Address)
                .NotEmpty().WithMessage("La dirección de la sucursal es obligatoria.")
                .MaximumLength(300).WithMessage("La dirección no puede exceder los 300 caracteres.");
        }
    }
}
