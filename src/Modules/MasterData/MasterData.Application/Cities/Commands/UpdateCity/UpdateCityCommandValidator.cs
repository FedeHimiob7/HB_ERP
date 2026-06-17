using FluentValidation;

namespace MasterData.Application.Cities.Commands.UpdateCity
{
    public class UpdateCityCommandValidator : AbstractValidator<UpdateCityCommand>
    {
        public UpdateCityCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("El identificador de la ciudad es obligatorio.");

            RuleFor(x => x.StateId)
                .NotEmpty().WithMessage("El identificador del estado/provincia es obligatorio.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre de la ciudad es obligatorio.")
                .MaximumLength(100).WithMessage("El nombre no puede exceder los 100 caracteres.");
        }
    }
}
