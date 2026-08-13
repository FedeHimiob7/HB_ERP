using MasterData.Application.Companies.Commands.UpdateCompany;
using MasterData.Domain.Enums;
using Xunit;

namespace Application.Tests
{
    public sealed class UpdateCompanyCommandValidatorTests
    {
        private readonly UpdateCompanyCommandValidator _validator = new();

        [Fact]
        public void Validate_WhenRifFormatIsInvalid_Fails()
        {
            var command = new UpdateCompanyCommand("RIF-INVALIDO", "Empresa Test", "Direccion Test", TaxpayerType.Ordinario);

            var result = _validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == nameof(UpdateCompanyCommand.Rif));
        }

        [Fact]
        public void Validate_WhenRifFormatIsValid_Succeeds()
        {
            var command = new UpdateCompanyCommand("J-401027631-4", "Empresa Test", "Direccion Test", TaxpayerType.Ordinario);

            var result = _validator.Validate(command);

            Assert.True(result.IsValid);
        }
    }
}
