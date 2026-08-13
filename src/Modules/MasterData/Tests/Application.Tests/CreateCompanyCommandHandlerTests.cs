using MasterData.Application.Companies.Commands.CreateCompany;
using MasterData.Application.Interfaces;
using MasterData.Domain.DomainErrors;
using MasterData.Domain.Enums;
using MasterData.Domain.Repositories;
using NSubstitute;
using Xunit;

namespace Application.Tests
{
    public sealed class CreateCompanyCommandHandlerTests
    {
        // Substitute.For<T>() crea la versión "falsa" de cada dependencia del handler.
        private readonly ICompanyRepository _repository = Substitute.For<ICompanyRepository>();
        private readonly IMasterDataUnitOfWork _unitOfWork = Substitute.For<IMasterDataUnitOfWork>();

        private CreateCompanyCommandHandler CreateHandler() => new(_repository, _unitOfWork);

        [Fact]
        public async Task Handle_WhenCompanyAlreadyExists_ReturnsAlreadyExists()
        {
            // Company es fila única por instalación (CompanyId.Singleton) — si ya existe una,
            // el handler debe cortar antes de intentar crear una segunda.
            _repository.ExistsAsync(Arg.Any<CancellationToken>()).Returns(true);

            var command = new CreateCompanyCommand("J-401027631-4", "Empresa Test", "Direccion Test", TaxpayerType.Ordinario);

            var result = await CreateHandler().Handle(command, CancellationToken.None);

            Assert.True(result.IsError);
            Assert.Equal(CompanyErrors.AlreadyExists.Code, result.FirstError.Code);
            // Nunca debería haber intentado persistir nada.
            await _repository.DidNotReceive().AddAsync(Arg.Any<MasterData.Domain.Entities.Company>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_WhenRifHasInvalidFormat_ReturnsInvalidRifFormat()
        {
            // No existe ninguna Company todavía, pero el Rif no cumple el patrón venezolano
            // (Company.Create lo valida internamente) — el handler debe propagar ese error de dominio.
            _repository.ExistsAsync(Arg.Any<CancellationToken>()).Returns(false);

            var command = new CreateCompanyCommand("RIF-INVALIDO", "Empresa Test", "Direccion Test", TaxpayerType.Ordinario);

            var result = await CreateHandler().Handle(command, CancellationToken.None);

            Assert.True(result.IsError);
            Assert.Equal(CompanyErrors.InvalidRifFormat.Code, result.FirstError.Code);
        }

        [Fact]
        public async Task Handle_WhenValid_PersistsCompanyAndReturnsId()
        {
            _repository.ExistsAsync(Arg.Any<CancellationToken>()).Returns(false);

            var command = new CreateCompanyCommand("J-401027631-4", "Empresa Test", "Direccion Test", TaxpayerType.Especial);

            var result = await CreateHandler().Handle(command, CancellationToken.None);

            Assert.False(result.IsError);
            Assert.NotEqual(Guid.Empty, result.Value);

            // Received(1) confirma COMPORTAMIENTO: se guardó una vez y se hizo commit una vez.
            await _repository.Received(1).AddAsync(Arg.Any<MasterData.Domain.Entities.Company>(), Arg.Any<CancellationToken>());
            await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }
    }
}
