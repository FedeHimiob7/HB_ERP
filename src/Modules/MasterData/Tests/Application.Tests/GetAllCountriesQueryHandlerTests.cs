using MasterData.Application.Countries.Queries.GetAllCountries;
using MasterData.Domain.Entities;
using MasterData.Domain.Repositories;
using NSubstitute;
using Xunit;

namespace Application.Tests
{
    public sealed class GetAllCountriesQueryHandlerTests
    {
        private readonly ICountryRepository _repository = Substitute.For<ICountryRepository>();

        private GetAllCountriesQueryHandler CreateHandler() => new(_repository);

        [Fact]
        public async Task Handle_MapsAllCountriesToResponse()
        {
            var countries = new List<Country>
            {
                Country.CreateExisting(Guid.NewGuid(), "Venezuela", isActive: true),
                Country.CreateExisting(Guid.NewGuid(), "Colombia", isActive: true),
            };
            _repository.GetAllAsync(Arg.Any<CancellationToken>()).Returns(countries);

            var result = await CreateHandler().Handle(new GetAllCountriesQuery(), CancellationToken.None);

            Assert.False(result.IsError);
            Assert.Equal(2, result.Value.Count);
            Assert.Contains(result.Value, c => c.Name == "Venezuela");
            Assert.Contains(result.Value, c => c.Name == "Colombia");
        }
    }
}
