using MasterData.Application.Units.Queries.GetAll;
using MasterData.Domain.Entities;
using MasterData.Domain.Repositories;
using NSubstitute;
using Xunit;

namespace Application.Tests
{
    public sealed class GetAllUnitsQueryHandlerTests
    {
        private readonly IUnitRepository _repository = Substitute.For<IUnitRepository>();

        private GetAllUnitsQueryHandler CreateHandler() => new(_repository);

        [Fact]
        public async Task Handle_MapsAllUnitsToResponse()
        {
            var units = new List<Unit> { Unit.Create("Kilogramo", "Unidad de peso").Value };
            _repository.GetAllAsync(Arg.Any<CancellationToken>()).Returns(units);

            var result = await CreateHandler().Handle(new GetAllUnitsQuery(), CancellationToken.None);

            Assert.False(result.IsError);
            Assert.Single(result.Value);
            Assert.Equal("Kilogramo", result.Value[0].Name);
        }
    }
}
