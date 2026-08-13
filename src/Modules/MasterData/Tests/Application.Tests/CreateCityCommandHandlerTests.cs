using MasterData.Application.Cities.Commands.CreateCity;
using MasterData.Application.Interfaces;
using MasterData.Domain.DomainErrors;
using MasterData.Domain.Entities;
using MasterData.Domain.Repositories;
using MasterData.Domain.VO;
using NSubstitute;
using Xunit;

namespace Application.Tests
{
    public sealed class CreateCityCommandHandlerTests
    {
        private readonly ICityRepository _cityRepository = Substitute.For<ICityRepository>();
        private readonly IStateRepository _stateRepository = Substitute.For<IStateRepository>();
        private readonly IMasterDataUnitOfWork _unitOfWork = Substitute.For<IMasterDataUnitOfWork>();
        private readonly Guid _stateGuid = Guid.NewGuid();

        private CreateCityCommandHandler CreateHandler() => new(_cityRepository, _stateRepository, _unitOfWork);

        [Fact]
        public async Task Handle_WhenStateDoesNotExist_ReturnsInvalidState()
        {
            _stateRepository.GetByIdAsync(Arg.Any<StateId>(), Arg.Any<CancellationToken>()).Returns((State?)null);

            var result = await CreateHandler().Handle(new CreateCityCommand(_stateGuid, "Los Teques"), CancellationToken.None);

            Assert.True(result.IsError);
            Assert.Equal(CityErrors.InvalidState.Code, result.FirstError.Code);
        }

        [Fact]
        public async Task Handle_WhenNameAlreadyExistsInState_ReturnsDuplicateName()
        {
            var state = State.CreateExisting(_stateGuid, Guid.NewGuid(), "MI", "Miranda", isActive: true);
            _stateRepository.GetByIdAsync(Arg.Any<StateId>(), Arg.Any<CancellationToken>()).Returns(state);
            // La unicidad de nombre de City es POR ESTADO, no global — por eso ExistsByNameInStateAsync
            // recibe el StateId, no solo el nombre.
            _cityRepository.ExistsByNameInStateAsync("Los Teques", Arg.Any<StateId>(), Arg.Any<CityId?>(), Arg.Any<CancellationToken>())
                .Returns(true);

            var result = await CreateHandler().Handle(new CreateCityCommand(_stateGuid, "Los Teques"), CancellationToken.None);

            Assert.True(result.IsError);
            Assert.Equal(CityErrors.DuplicateName.Code, result.FirstError.Code);
        }

        [Fact]
        public async Task Handle_WhenValid_PersistsCityAndReturnsId()
        {
            var state = State.CreateExisting(_stateGuid, Guid.NewGuid(), "MI", "Miranda", isActive: true);
            _stateRepository.GetByIdAsync(Arg.Any<StateId>(), Arg.Any<CancellationToken>()).Returns(state);
            _cityRepository.ExistsByNameInStateAsync(Arg.Any<string>(), Arg.Any<StateId>(), Arg.Any<CityId?>(), Arg.Any<CancellationToken>())
                .Returns(false);

            var result = await CreateHandler().Handle(new CreateCityCommand(_stateGuid, "Los Teques"), CancellationToken.None);

            Assert.False(result.IsError);
            Assert.NotEqual(Guid.Empty, result.Value);

            await _cityRepository.Received(1).AddAsync(Arg.Any<City>(), Arg.Any<CancellationToken>());
            await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }
    }
}
