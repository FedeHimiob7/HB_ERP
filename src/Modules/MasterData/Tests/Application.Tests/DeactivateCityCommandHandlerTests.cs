using MasterData.Application.Cities.Commands.DeactivateCity;
using MasterData.Application.Interfaces;
using MasterData.Domain.DomainErrors;
using MasterData.Domain.Entities;
using MasterData.Domain.Repositories;
using MasterData.Domain.VO;
using NSubstitute;
using Xunit;

namespace Application.Tests
{
    public sealed class DeactivateCityCommandHandlerTests
    {
        private readonly ICityRepository _repository = Substitute.For<ICityRepository>();
        private readonly IMasterDataUnitOfWork _unitOfWork = Substitute.For<IMasterDataUnitOfWork>();
        private readonly Guid _cityGuid = Guid.NewGuid();

        private DeactivateCityCommandHandler CreateHandler() => new(_repository, _unitOfWork);

        [Fact]
        public async Task Handle_WhenCityDoesNotExist_ReturnsNotFound()
        {
            _repository.GetByIdAsync(Arg.Any<CityId>(), Arg.Any<CancellationToken>()).Returns((City?)null);

            var result = await CreateHandler().Handle(new DeactivateCityCommand(_cityGuid), CancellationToken.None);

            Assert.True(result.IsError);
            Assert.Equal(CityErrors.NotFound.Code, result.FirstError.Code);
        }

        [Fact]
        public async Task Handle_WhenValid_DeactivatesAndPersists()
        {
            var city = City.CreateExisting(_cityGuid, Guid.NewGuid(), "Los Teques", isActive: true);
            _repository.GetByIdAsync(Arg.Any<CityId>(), Arg.Any<CancellationToken>()).Returns(city);

            var result = await CreateHandler().Handle(new DeactivateCityCommand(_cityGuid), CancellationToken.None);

            Assert.False(result.IsError);
            Assert.False(city.IsActive);

            await _repository.Received(1).UpdateAsync(city, Arg.Any<CancellationToken>());
            await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }
    }
}
