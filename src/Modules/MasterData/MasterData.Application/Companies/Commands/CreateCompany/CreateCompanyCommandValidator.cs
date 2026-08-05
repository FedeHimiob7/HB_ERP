using FluentValidation;

namespace MasterData.Application.Companies.Commands.CreateCompany
{
    public class CreateCompanyCommandValidator : AbstractValidator<CreateCompanyCommand>
    {
        public CreateCompanyCommandValidator()
        {
            RuleFor(x => x.Rif)
                .NotEmpty().WithMessage("El RIF de la empresa es obligatorio.")
                .MaximumLength(20).WithMessage("El RIF no puede exceder los 20 caracteres.");

            RuleFor(x => x.LegalName)
                .NotEmpty().WithMessage("La razón social es obligatoria.")
                .MaximumLength(200).WithMessage("La razón social no puede exceder los 200 caracteres.");

            RuleFor(x => x.RegisteredAddress)
                .NotEmpty().WithMessage("El domicilio fiscal es obligatorio.")
                .MaximumLength(300).WithMessage("El domicilio fiscal no puede exceder los 300 caracteres.");

            RuleFor(x => x.TaxpayerType).IsInEnum();
        }
    }
}
