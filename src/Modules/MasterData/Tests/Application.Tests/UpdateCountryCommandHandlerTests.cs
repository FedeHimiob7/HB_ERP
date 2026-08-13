using MasterData.Application.Countries.Commands.UpdateCountry;
using MasterData.Application.Interfaces;
using MasterData.Domain.DomainErrors;
using MasterData.Domain.Entities;
using MasterData.Domain.Repositories;
using MasterData.Domain.VO;
using NSubstitute;
using Xunit;

namespace Application.Tests
{
    public sealed class UpdateCountryCommandHandlerTests
    {
        private readonly ICountryRepository _repository = Substitute.For<ICountryRepository>();
        private readonly IMasterDataUnitOfWork _unitOfWork = Substitute.For<IMasterDataUnitOfWork>();
        private readonly Guid _countryGuid = Guid.NewGuid();

        private UpdateCountryCommandHandler CreateHandler() => new(_repository, _unitOfWork);

        [Fact]
        public async Task Handle_WhenCountryDoesNotExist_ReturnsNotFound()
        {
            _repository.GetByIdAsync(Arg.Any<CountryId>(), Arg.Any<CancellationToken>()).Returns((Country?)null);

            var result = await CreateHandler().Handle(new UpdateCountryCommand(_countryGuid, "Colombia"), CancellationToken.None);

            Assert.True(result.IsError);
            // El handler arma el Error.NotFound "a mano" en vez de usar CountryErrors.NotFound,
            // pero el código es idéntico — lo comparamos igual para blindar el comportamiento observable.
            Assert.Equal(CountryErrors.NotFound.Code, result.FirstError.Code);
        }

        [Fact]
        public async Task Handle_WhenValid_UpdatesAndReturnsResponse()
        {
            var country = Country.CreateExisting(_countryGuid, "Nombre Viejo", isActive: true);
            _repository.GetByIdAsync(Arg.Any<CountryId>(), Arg.Any<CancellationToken>()).Returns(country);

            var result = await CreateHandler().Handle(new UpdateCountryCommand(_countryGuid, "Nombre Nuevo"), CancellationToken.None);

            Assert.False(result.IsError);
            Assert.Equal("Nombre Nuevo", result.Value.Name);

            await _repository.Received(1).UpdateAsync(country, Arg.Any<CancellationToken>());
            await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }
    }
}
