using MasterData.Application.Cities.Queries.GetById;
using MasterData.Domain.DomainErrors;
using MasterData.Domain.Entities;
using MasterData.Domain.Repositories;
using MasterData.Domain.VO;
using NSubstitute;
using Xunit;

namespace Application.Tests
{
    public sealed class GetCityByIdQueryHandlerTests
    {
        private readonly ICityRepository _repository = Substitute.For<ICityRepository>();
        private readonly Guid _cityGuid = Guid.NewGuid();

        private GetCityByIdQueryHandler CreateHandler() => new(_repository);

        [Fact]
        public async Task Handle_WhenNotFound_ReturnsNotFound()
        {
            _repository.GetByIdAsync(Arg.Any<CityId>(), Arg.Any<CancellationToken>()).Returns((City?)null);

            var result = await CreateHandler().Handle(new GetCityByIdQuery(_cityGuid), CancellationToken.None);

            Assert.True(result.IsError);
            Assert.Equal(CityErrors.NotFound.Code, result.FirstError.Code);
        }

        [Fact]
        public async Task Handle_WhenFound_ReturnsResponse()
        {
            var city = City.CreateExisting(_cityGuid, Guid.NewGuid(), "Los Teques", isActive: true);
            _repository.GetByIdAsync(Arg.Any<CityId>(), Arg.Any<CancellationToken>()).Returns(city);

            var result = await CreateHandler().Handle(new GetCityByIdQuery(_cityGuid), CancellationToken.None);

            Assert.False(result.IsError);
            Assert.Equal(_cityGuid, result.Value.Id);
        }
    }
}
