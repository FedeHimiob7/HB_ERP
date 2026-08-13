using MasterData.Application.States.Queries.GetAll;
using MasterData.Domain.Entities;
using MasterData.Domain.Repositories;
using MasterData.Domain.VO;
using NSubstitute;
using Xunit;

namespace Application.Tests
{
    public sealed class GetAllStatesQueryHandlerTests
    {
        private readonly IStateRepository _repository = Substitute.For<IStateRepository>();

        private GetAllStatesQueryHandler CreateHandler() => new(_repository);

        [Fact]
        public async Task Handle_WhenCountryIdProvided_PassesItToRepository()
        {
            var countryGuid = Guid.NewGuid();
            _repository.GetAllAsync(Arg.Any<CountryId?>(), Arg.Any<CancellationToken>()).Returns(new List<State>());

            await CreateHandler().Handle(new GetAllStatesQuery(countryGuid), CancellationToken.None);

            await _repository.Received(1).GetAllAsync(
                Arg.Is<CountryId?>(id => id == CountryId.Create(countryGuid)),
                Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_MapsStatesToResponse()
        {
            var countryGuid = Guid.NewGuid();
            var states = new List<State> { State.CreateExisting(Guid.NewGuid(), countryGuid, "MI", "Miranda", isActive: true) };
            _repository.GetAllAsync(Arg.Any<CountryId?>(), Arg.Any<CancellationToken>()).Returns(states);

            var result = await CreateHandler().Handle(new GetAllStatesQuery(null), CancellationToken.None);

            Assert.False(result.IsError);
            Assert.Single(result.Value);
            Assert.Equal("MI", result.Value[0].Code);
        }
    }
}
