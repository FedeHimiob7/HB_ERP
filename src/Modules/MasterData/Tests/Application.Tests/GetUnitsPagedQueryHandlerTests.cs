using MasterData.Application.Units.Queries.GetPaged;
using MasterData.Domain.Entities;
using MasterData.Domain.Repositories;
using MasterData.Domain.SearchParametersModel;
using NSubstitute;
using Xunit;

namespace Application.Tests
{
    public sealed class GetUnitsPagedQueryHandlerTests
    {
        private readonly IUnitRepository _repository = Substitute.For<IUnitRepository>();

        private GetUnitsPagedQueryHandler CreateHandler() => new(_repository);

        [Fact]
        public async Task Handle_ReturnsPagedResultUsingFilterPassedByCaller()
        {
            var units = new List<Unit> { Unit.Create("Kilogramo", "Unidad de peso").Value };
            var filter = new UnitFilter(1, 10, "Kilo");
            _repository.GetPagedAsync(filter, Arg.Any<CancellationToken>()).Returns((units, 2));

            var result = await CreateHandler().Handle(new GetUnitsPagedQuery(filter), CancellationToken.None);

            Assert.False(result.IsError);
            Assert.Single(result.Value.Items);
            Assert.Equal(2, result.Value.TotalCount);
        }
    }
}
