using MasterData.Application.States.Queries.GetPaged;
using MasterData.Domain.Entities;
using MasterData.Domain.Repositories;
using MasterData.Domain.SearchParametersModel;
using NSubstitute;
using Xunit;

namespace Application.Tests
{
    public sealed class GetStatesPagedQueryHandlerTests
    {
        private readonly IStateRepository _repository = Substitute.For<IStateRepository>();

        private GetStatesPagedQueryHandler CreateHandler() => new(_repository);

        [Fact]
        public async Task Handle_BuildsFilterWithCountryIdAndReturnsPagedResult()
        {
            var countryGuid = Guid.NewGuid();
            var states = new List<State> { State.CreateExisting(Guid.NewGuid(), countryGuid, "MI", "Miranda", isActive: true) };
            _repository.GetPagedAsync(Arg.Any<StateFilter>(), Arg.Any<CancellationToken>()).Returns((states, 24));

            var query = new GetStatesPagedQuery(1, 10, countryGuid, "Mir");
            var result = await CreateHandler().Handle(query, CancellationToken.None);

            Assert.False(result.IsError);
            Assert.Single(result.Value.Items);
            Assert.Equal(24, result.Value.TotalCount);

            await _repository.Received(1).GetPagedAsync(
                Arg.Is<StateFilter>(f => f.CountryId != null && f.CountryId.Value.Value == countryGuid && f.SearchTerm == "Mir"),
                Arg.Any<CancellationToken>());
        }
    }
}
