using MasterData.Application.Interfaces;
using MasterData.Application.States.Commands.CreateState;
using MasterData.Domain.DomainErrors;
using MasterData.Domain.Entities;
using MasterData.Domain.Repositories;
using MasterData.Domain.VO;
using NSubstitute;
using Xunit;

namespace Application.Tests
{
    public sealed class CreateStateCommandHandlerTests
    {
        private readonly IStateRepository _stateRepository = Substitute.For<IStateRepository>();
        private readonly ICountryRepository _countryRepository = Substitute.For<ICountryRepository>();
        private readonly IMasterDataUnitOfWork _unitOfWork = Substitute.For<IMasterDataUnitOfWork>();
        private readonly Guid _countryGuid = Guid.NewGuid();

        private CreateStateCommandHandler CreateHandler() => new(_stateRepository, _countryRepository, _unitOfWork);

        [Fact]
        public async Task Handle_WhenCountryDoesNotExist_ReturnsInvalidCountry()
        {
            _countryRepository.GetByIdAsync(Arg.Any<CountryId>(), Arg.Any<CancellationToken>()).Returns((Country?)null);

            var command = new CreateStateCommand(_countryGuid, "MI", "Miranda");
            var result = await CreateHandler().Handle(command, CancellationToken.None);

            Assert.True(result.IsError);
            Assert.Equal(StateErrors.InvalidCountry.Code, result.FirstError.Code);
        }

        [Fact]
        public async Task Handle_WhenCodeAlreadyExists_ReturnsDuplicateCode()
        {
            var country = Country.CreateExisting(_countryGuid, "Venezuela", isActive: true);
            _countryRepository.GetByIdAsync(Arg.Any<CountryId>(), Arg.Any<CancellationToken>()).Returns(country);
            _stateRepository.ExistsByCodeAsync("MI", Arg.Any<CancellationToken>()).Returns(true);

            var command = new CreateStateCommand(_countryGuid, "MI", "Miranda");
            var result = await CreateHandler().Handle(command, CancellationToken.None);

            Assert.True(result.IsError);
            Assert.Equal(StateErrors.DuplicateCode.Code, result.FirstError.Code);
        }

        [Fact]
        public async Task Handle_WhenValid_PersistsStateAndReturnsId()
        {
            var country = Country.CreateExisting(_countryGuid, "Venezuela", isActive: true);
            _countryRepository.GetByIdAsync(Arg.Any<CountryId>(), Arg.Any<CancellationToken>()).Returns(country);
            _stateRepository.ExistsByCodeAsync("MI", Arg.Any<CancellationToken>()).Returns(false);

            var command = new CreateStateCommand(_countryGuid, "MI", "Miranda");
            var result = await CreateHandler().Handle(command, CancellationToken.None);

            Assert.False(result.IsError);
            Assert.NotEqual(Guid.Empty, result.Value);

            await _stateRepository.Received(1).AddAsync(Arg.Any<State>(), Arg.Any<CancellationToken>());
            await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }
    }
}
