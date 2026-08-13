using MasterData.Application.Cities.Queries.GetPaged;
using MasterData.Domain.Entities;
using MasterData.Domain.Repositories;
using MasterData.Domain.SearchParametersModel;
using NSubstitute;
using Xunit;

namespace Application.Tests
{
    public sealed class GetCitiesPagedQueryHandlerTests
    {
        private readonly ICityRepository _repository = Substitute.For<ICityRepository>();

        private GetCitiesPagedQueryHandler CreateHandler() => new(_repository);

        [Fact]
        public async Task Handle_ReturnsPagedResultUsingFilterPassedByCaller()
        {
            // A diferencia de Branch/State/FiscalTerminal (arman el filtro adentro del handler),
            // GetCitiesPagedQuery recibe el CityFilter ya armado — el handler solo lo reenvía tal cual.
            var cities = new List<City> { City.CreateExisting(Guid.NewGuid(), Guid.NewGuid(), "Los Teques", isActive: true) };
            var filter = new CityFilter(1, 10, "Teques");
            _repository.GetPagedAsync(filter, Arg.Any<CancellationToken>()).Returns((cities, 3));

            var result = await CreateHandler().Handle(new GetCitiesPagedQuery(filter), CancellationToken.None);

            Assert.False(result.IsError);
            Assert.Single(result.Value.Items);
            Assert.Equal(3, result.Value.TotalCount);
        }
    }
}
