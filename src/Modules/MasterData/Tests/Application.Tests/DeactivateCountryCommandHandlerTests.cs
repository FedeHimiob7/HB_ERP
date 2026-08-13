using MasterData.Application.Countries.Commands.DeleteCountry;
using MasterData.Application.Interfaces;
using MasterData.Domain.DomainErrors;
using MasterData.Domain.Entities;
using MasterData.Domain.Repositories;
using MasterData.Domain.VO;
using NSubstitute;
using Xunit;

namespace Application.Tests
{
    public sealed class DeactivateCountryCommandHandlerTests
    {
        private readonly ICountryRepository _repository = Substitute.For<ICountryRepository>();
        private readonly IMasterDataUnitOfWork _unitOfWork = Substitute.For<IMasterDataUnitOfWork>();
        private readonly Guid _countryGuid = Guid.NewGuid();

        private DeactivateCountryCommandHandler CreateHandler() => new(_repository, _unitOfWork);

        [Fact]
        public async Task Handle_WhenCountryDoesNotExist_ReturnsNotFound()
        {
            _repository.GetByIdAsync(Arg.Any<CountryId>(), Arg.Any<CancellationToken>()).Returns((Country?)null);

            var result = await CreateHandler().Handle(new DeactivateCountryCommand(_countryGuid), CancellationToken.None);

            Assert.True(result.IsError);
            Assert.Equal(CountryErrors.NotFound.Code, result.FirstError.Code);
        }

        [Fact]
        public async Task Handle_WhenValid_DeactivatesAndPersists()
        {
            var country = Country.CreateExisting(_countryGuid, "Venezuela", isActive: true);
            _repository.GetByIdAsync(Arg.Any<CountryId>(), Arg.Any<CancellationToken>()).Returns(country);

            var result = await CreateHandler().Handle(new DeactivateCountryCommand(_countryGuid), CancellationToken.None);

            Assert.False(result.IsError);
            Assert.False(country.IsActive);

            await _repository.Received(1).UpdateAsync(country, Arg.Any<CancellationToken>());
            await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }
    }
}
