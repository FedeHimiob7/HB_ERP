using MasterData.Application.Interfaces;
using MasterData.Application.States.Commands.UpdateState;
using MasterData.Domain.DomainErrors;
using MasterData.Domain.Entities;
using MasterData.Domain.Repositories;
using MasterData.Domain.VO;
using NSubstitute;
using Xunit;

namespace Application.Tests
{
    public sealed class UpdateStateCommandHandlerTests
    {
        private readonly IStateRepository _stateRepository = Substitute.For<IStateRepository>();
        private readonly ICountryRepository _countryRepository = Substitute.For<ICountryRepository>();
        private readonly IMasterDataUnitOfWork _unitOfWork = Substitute.For<IMasterDataUnitOfWork>();
        private readonly Guid _stateGuid = Guid.NewGuid();
        private readonly Guid _countryGuid = Guid.NewGuid();

        private UpdateStateCommandHandler CreateHandler() => new(_stateRepository, _countryRepository, _unitOfWork);

        [Fact]
        public async Task Handle_WhenStateDoesNotExist_ReturnsNotFound()
        {
            _stateRepository.GetByIdAsync(Arg.Any<StateId>(), Arg.Any<CancellationToken>()).Returns((State?)null);

            var command = new UpdateStateCommand(_stateGuid, _countryGuid, "MI", "Miranda");
            var result = await CreateHandler().Handle(command, CancellationToken.None);

            Assert.True(result.IsError);
            Assert.Equal(StateErrors.NotFound.Code, result.FirstError.Code);
        }

        [Fact]
        public async Task Handle_WhenCountryChangesToOneThatDoesNotExist_ReturnsInvalidCountry()
        {
            var otherCountryGuid = Guid.NewGuid();
            var state = State.CreateExisting(_stateGuid, otherCountryGuid, "MI", "Miranda Vieja", isActive: true);
            _stateRepository.GetByIdAsync(Arg.Any<StateId>(), Arg.Any<CancellationToken>()).Returns(state);
            // El comando pide mover el estado a un país distinto al que ya tenía, y ese país no existe.
            _countryRepository.GetByIdAsync(Arg.Any<CountryId>(), Arg.Any<CancellationToken>()).Returns((Country?)null);

            var command = new UpdateStateCommand(_stateGuid, _countryGuid, "MI", "Miranda");
            var result = await CreateHandler().Handle(command, CancellationToken.None);

            Assert.True(result.IsError);
            Assert.Equal(StateErrors.InvalidCountry.Code, result.FirstError.Code);
        }

        [Fact]
        public async Task Handle_WhenCountryIsUnchanged_DoesNotCheckCountryExistence()
        {
            // Optimización del handler: si CountryId no cambia, no hace falta volver a validar que
            // el país exista (ya se sabe que existía cuando se creó el estado).
            var state = State.CreateExisting(_stateGuid, _countryGuid, "MI", "Miranda Vieja", isActive: true);
            _stateRepository.GetByIdAsync(Arg.Any<StateId>(), Arg.Any<CancellationToken>()).Returns(state);
            _stateRepository.ExistsByCodeAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(false);

            var command = new UpdateStateCommand(_stateGuid, _countryGuid, "MI", "Miranda");
            var result = await CreateHandler().Handle(command, CancellationToken.None);

            Assert.False(result.IsError);
            await _countryRepository.DidNotReceive().GetByIdAsync(Arg.Any<CountryId>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_WhenCodeChangesToOneAlreadyInUse_ReturnsDuplicateCode()
        {
            var state = State.CreateExisting(_stateGuid, _countryGuid, "MI", "Miranda Vieja", isActive: true);
            _stateRepository.GetByIdAsync(Arg.Any<StateId>(), Arg.Any<CancellationToken>()).Returns(state);
            _stateRepository.ExistsByCodeAsync("AR", Arg.Any<CancellationToken>()).Returns(true);

            var command = new UpdateStateCommand(_stateGuid, _countryGuid, "AR", "Aragua");
            var result = await CreateHandler().Handle(command, CancellationToken.None);

            Assert.True(result.IsError);
            Assert.Equal(StateErrors.DuplicateCode.Code, result.FirstError.Code);
        }

        [Fact]
        public async Task Handle_WhenValid_UpdatesAndReturnsResponse()
        {
            var state = State.CreateExisting(_stateGuid, _countryGuid, "MI", "Miranda Vieja", isActive: true);
            _stateRepository.GetByIdAsync(Arg.Any<StateId>(), Arg.Any<CancellationToken>()).Returns(state);

            var command = new UpdateStateCommand(_stateGuid, _countryGuid, "MI", "Miranda Nueva");
            var result = await CreateHandler().Handle(command, CancellationToken.None);

            Assert.False(result.IsError);
            Assert.Equal("Miranda Nueva", result.Value.Name);

            await _stateRepository.Received(1).UpdateAsync(state, Arg.Any<CancellationToken>());
            await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }
    }
}
