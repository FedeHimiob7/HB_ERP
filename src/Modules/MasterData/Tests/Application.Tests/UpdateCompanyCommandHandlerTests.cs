using MasterData.Application.Companies.Commands.UpdateCompany;
using MasterData.Application.Interfaces;
using MasterData.Domain.DomainErrors;
using MasterData.Domain.Entities;
using MasterData.Domain.Enums;
using MasterData.Domain.Repositories;
using NSubstitute;
using Xunit;

namespace Application.Tests
{
    public sealed class UpdateCompanyCommandHandlerTests
    {
        private readonly ICompanyRepository _repository = Substitute.For<ICompanyRepository>();
        private readonly IMasterDataUnitOfWork _unitOfWork = Substitute.For<IMasterDataUnitOfWork>();

        private UpdateCompanyCommandHandler CreateHandler() => new(_repository, _unitOfWork);

        [Fact]
        public async Task Handle_WhenCompanyNotConfigured_ReturnsNotConfigured()
        {
            // Todavía no se creó ninguna Company en esta instalación (GetSingletonAsync devuelve null).
            _repository.GetSingletonAsync(Arg.Any<CancellationToken>()).Returns((Company?)null);

            var command = new UpdateCompanyCommand("J-401027631-4", "Empresa Test", "Direccion Test", TaxpayerType.Ordinario);

            var result = await CreateHandler().Handle(command, CancellationToken.None);

            Assert.True(result.IsError);
            Assert.Equal(CompanyErrors.NotConfigured.Code, result.FirstError.Code);
        }

        [Fact]
        public async Task Handle_WhenValid_UpdatesAndReturnsUpdatedResponse()
        {
            // CreateExisting reconstruye la entidad "como si ya estuviera en la base",
            // con datos viejos que el comando debería reemplazar.
            var existingCompany = Company.CreateExisting("J-401027631-4", "Nombre Viejo", "Direccion Vieja", TaxpayerType.Formal);
            _repository.GetSingletonAsync(Arg.Any<CancellationToken>()).Returns(existingCompany);

            var command = new UpdateCompanyCommand("J-401027631-4", "Nombre Nuevo", "Direccion Nueva", TaxpayerType.Especial);

            var result = await CreateHandler().Handle(command, CancellationToken.None);

            Assert.False(result.IsError);
            Assert.Equal("Nombre Nuevo", result.Value.LegalName);
            Assert.Equal("Direccion Nueva", result.Value.RegisteredAddress);
            Assert.Equal(TaxpayerType.Especial, result.Value.TaxpayerType);

            await _repository.Received(1).UpdateAsync(existingCompany, Arg.Any<CancellationToken>());
            await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }
    }
}
