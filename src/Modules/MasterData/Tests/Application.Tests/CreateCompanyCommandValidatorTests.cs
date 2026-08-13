using MasterData.Application.Companies.Commands.CreateCompany;
using MasterData.Domain.Enums;
using Xunit;

namespace Application.Tests
{
    public sealed class CreateCompanyCommandValidatorTests
    {
        private readonly CreateCompanyCommandValidator _validator = new();

        [Theory]
        [InlineData("RIF-INVALIDO")]
        [InlineData("J401027631-4")]
        [InlineData("X-401027631-4")]
        public void Validate_WhenRifFormatIsInvalid_Fails(string invalidRif)
        {
            // Letra fuera de [VEJPG], sin guiones, o formato totalmente distinto: debe fallar el
            // Matches() nuevo, ANTES de que la request llegue al handler/dominio.
            var command = new CreateCompanyCommand(invalidRif, "Empresa Test", "Direccion Test", TaxpayerType.Ordinario);

            var result = _validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateCompanyCommand.Rif));
        }

        [Theory]
        [InlineData("J-401027631-4")]
        [InlineData("j-401027631-4")]
        [InlineData("V-12345678-9")]
        public void Validate_WhenRifFormatIsValid_Succeeds(string validRif)
        {
            // Formato venezolano correcto (letra V/E/J/P/G, mayúscula o minúscula, guion, 8-9 dígitos, guion, 1 dígito).
            var command = new CreateCompanyCommand(validRif, "Empresa Test", "Direccion Test", TaxpayerType.Ordinario);

            var result = _validator.Validate(command);

            Assert.True(result.IsValid);
        }
    }
}
