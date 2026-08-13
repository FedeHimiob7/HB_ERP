using MasterData.Application.Cities.Commands.UpdateCity;
using MasterData.Application.Interfaces;
using MasterData.Domain.DomainErrors;
using MasterData.Domain.Entities;
using MasterData.Domain.Repositories;
using MasterData.Domain.VO;
using NSubstitute;
using Xunit;

namespace Application.Tests
{
    public sealed class UpdateCityCommandHandlerTests
    {
        private readonly ICityRepository _cityRepository = Substitute.For<ICityRepository>();
        private readonly IStateRepository _stateRepository = Substitute.For<IStateRepository>();
        private readonly IMasterDataUnitOfWork _unitOfWork = Substitute.For<IMasterDataUnitOfWork>();
        private readonly Guid _cityGuid = Guid.NewGuid();
        private readonly Guid _stateGuid = Guid.NewGuid();

        private UpdateCityCommandHandler CreateHandler() => new(_cityRepository, _stateRepository, _unitOfWork);

        [Fact]
        public async Task Handle_WhenCityDoesNotExist_ReturnsNotFound()
        {
            _cityRepository.GetByIdAsync(Arg.Any<CityId>(), Arg.Any<CancellationToken>()).Returns((City?)null);

            var result = await CreateHandler().Handle(new UpdateCityCommand(_cityGuid, _stateGuid, "Los Teques"), CancellationToken.None);

            Assert.True(result.IsError);
            Assert.Equal(CityErrors.NotFound.Code, result.FirstError.Code);
        }

        [Fact]
        public async Task Handle_WhenStateDoesNotExist_ReturnsInvalidState()
        {
            var city = City.CreateExisting(_cityGuid, _stateGuid, "Los Teques", isActive: true);
            _cityRepository.GetByIdAsync(Arg.Any<CityId>(), Arg.Any<CancellationToken>()).Returns(city);
            // A diferencia de State (que solo revalida el país si cambia), City SIEMPRE revalida el
            // estado en cada Update — confirmamos que corta acá aunque sea el mismo StateId de siempre.
            _stateRepository.GetByIdAsync(Arg.Any<StateId>(), Arg.Any<CancellationToken>()).Returns((State?)null);

            var result = await CreateHandler().Handle(new UpdateCityCommand(_cityGuid, _stateGuid, "Los Teques"), CancellationToken.None);

            Assert.True(result.IsError);
            Assert.Equal(CityErrors.InvalidState.Code, result.FirstError.Code);
        }

        [Fact]
        public async Task Handle_WhenNameAlreadyExistsInState_ReturnsDuplicateName()
        {
            var city = City.CreateExisting(_cityGuid, _stateGuid, "Los Teques", isActive: true);
            _cityRepository.GetByIdAsync(Arg.Any<CityId>(), Arg.Any<CancellationToken>()).Returns(city);
            var state = State.CreateExisting(_stateGuid, Guid.NewGuid(), "MI", "Miranda", isActive: true);
            _stateRepository.GetByIdAsync(Arg.Any<StateId>(), Arg.Any<CancellationToken>()).Returns(state);
            _cityRepository.ExistsByNameInStateAsync("San Antonio", Arg.Any<StateId>(), city.Id, Arg.Any<CancellationToken>())
                .Returns(true);

            var result = await CreateHandler().Handle(new UpdateCityCommand(_cityGuid, _stateGuid, "San Antonio"), CancellationToken.None);

            Assert.True(result.IsError);
            Assert.Equal(CityErrors.DuplicateName.Code, result.FirstError.Code);
        }

        [Fact]
        public async Task Handle_WhenValid_UpdatesAndReturnsResponse()
        {
            var city = City.CreateExisting(_cityGuid, _stateGuid, "Los Teques", isActive: true);
            _cityRepository.GetByIdAsync(Arg.Any<CityId>(), Arg.Any<CancellationToken>()).Returns(city);
            var state = State.CreateExisting(_stateGuid, Guid.NewGuid(), "MI", "Miranda", isActive: true);
            _stateRepository.GetByIdAsync(Arg.Any<StateId>(), Arg.Any<CancellationToken>()).Returns(state);
            _cityRepository.ExistsByNameInStateAsync(Arg.Any<string>(), Arg.Any<StateId>(), Arg.Any<CityId?>(), Arg.Any<CancellationToken>())
                .Returns(false);

            var result = await CreateHandler().Handle(new UpdateCityCommand(_cityGuid, _stateGuid, "San Antonio"), CancellationToken.None);

            Assert.False(result.IsError);
            Assert.Equal("San Antonio", result.Value.Name);

            await _cityRepository.Received(1).UpdateAsync(city, Arg.Any<CancellationToken>());
            await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }
    }
}
