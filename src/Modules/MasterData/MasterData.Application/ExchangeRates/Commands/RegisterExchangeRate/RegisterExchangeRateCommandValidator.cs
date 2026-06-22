using FluentValidation;

namespace MasterData.Application.ExchangeRates.Commands.RegisterExchangeRate
{
    public class RegisterExchangeRateCommandValidator : AbstractValidator<RegisterExchangeRateCommand>
    {
        public RegisterExchangeRateCommandValidator()
        {
            RuleFor(x => x.Rate)
                .GreaterThan(0).WithMessage("La tasa de cambio debe ser mayor a cero.");

            RuleFor(x => x.Source)
                .IsInEnum().WithMessage("Fuente de tasa inválida.");
        }
    }
}
