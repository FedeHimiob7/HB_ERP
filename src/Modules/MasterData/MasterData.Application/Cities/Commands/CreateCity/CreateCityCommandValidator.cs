using FluentValidation;

namespace MasterData.Application.Cities.Commands.CreateCity
{
    public class CreateCityCommandValidator : AbstractValidator<CreateCityCommand>
    {
        public CreateCityCommandValidator()
        {
            RuleFor(x => x.StateId)
                .NotEmpty().WithMessage("El identificador del estado/provincia es obligatorio.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre de la ciudad es obligatorio.")
                .MaximumLength(100).WithMessage("El nombre no puede exceder los 100 caracteres.");
        }
    }
}
