using MasterData.Application.Countries.Queries.GetCountryById;
using MasterData.Domain.DomainErrors;
using MasterData.Domain.Entities;
using MasterData.Domain.Repositories;
using MasterData.Domain.VO;
using NSubstitute;
using Xunit;

namespace Application.Tests
{
    public sealed class GetCountryByIdQueryHandlerTests
    {
        private readonly ICountryRepository _repository = Substitute.For<ICountryRepository>();
        private readonly Guid _countryGuid = Guid.NewGuid();

        private GetCountryByIdQueryHandler CreateHandler() => new(_repository);

        [Fact]
        public async Task Handle_WhenNotFound_ReturnsNotFound()
        {
            _repository.GetByIdAsync(Arg.Any<CountryId>(), Arg.Any<CancellationToken>()).Returns((Country?)null);

            var result = await CreateHandler().Handle(new GetCountryByIdQuery(_countryGuid), CancellationToken.None);

            Assert.True(result.IsError);
            Assert.Equal(CountryErrors.NotFound.Code, result.FirstError.Code);
        }

        [Fact]
        public async Task Handle_WhenFound_ReturnsResponse()
        {
            var country = Country.CreateExisting(_countryGuid, "Venezuela", isActive: true);
            _repository.GetByIdAsync(Arg.Any<CountryId>(), Arg.Any<CancellationToken>()).Returns(country);

            var result = await CreateHandler().Handle(new GetCountryByIdQuery(_countryGuid), CancellationToken.None);

            Assert.False(result.IsError);
            Assert.Equal(_countryGuid, result.Value.Id);
            Assert.Equal("Venezuela", result.Value.Name);
        }
    }
}
