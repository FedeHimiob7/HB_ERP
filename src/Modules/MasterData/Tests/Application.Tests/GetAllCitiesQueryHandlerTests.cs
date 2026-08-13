using MasterData.Application.Cities.Queries.GetAll;
using MasterData.Domain.Entities;
using MasterData.Domain.Repositories;
using NSubstitute;
using Xunit;

namespace Application.Tests
{
    public sealed class GetAllCitiesQueryHandlerTests
    {
        private readonly ICityRepository _repository = Substitute.For<ICityRepository>();

        private GetAllCitiesQueryHandler CreateHandler() => new(_repository);

        [Fact]
        public async Task Handle_MapsAllCitiesToResponse()
        {
            var cities = new List<City> { City.CreateExisting(Guid.NewGuid(), Guid.NewGuid(), "Los Teques", isActive: true) };
            _repository.GetAllAsync(Arg.Any<CancellationToken>()).Returns(cities);

            var result = await CreateHandler().Handle(new GetAllCitiesQuery(), CancellationToken.None);

            Assert.False(result.IsError);
            Assert.Single(result.Value);
            Assert.Equal("Los Teques", result.Value[0].Name);
        }
    }
}
