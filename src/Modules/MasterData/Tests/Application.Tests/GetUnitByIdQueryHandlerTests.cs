using MasterData.Application.Units.Queries.GetById;
using MasterData.Domain.DomainErrors;
using MasterData.Domain.Entities;
using MasterData.Domain.Repositories;
using MasterData.Domain.VO;
using NSubstitute;
using Xunit;

namespace Application.Tests
{
    public sealed class GetUnitByIdQueryHandlerTests
    {
        private readonly IUnitRepository _repository = Substitute.For<IUnitRepository>();
        private readonly Guid _unitGuid = Guid.NewGuid();

        private GetUnitByIdQueryHandler CreateHandler() => new(_repository);

        [Fact]
        public async Task Handle_WhenNotFound_ReturnsNotFound()
        {
            _repository.GetByIdAsync(Arg.Any<UnitId>(), Arg.Any<CancellationToken>()).Returns((Unit?)null);

            var result = await CreateHandler().Handle(new GetUnitByIdQuery(_unitGuid), CancellationToken.None);

            Assert.True(result.IsError);
            Assert.Equal(UnitErrors.NotFound.Code, result.FirstError.Code);
        }

        [Fact]
        public async Task Handle_WhenFound_ReturnsResponse()
        {
            var unit = Unit.Create("Kilogramo", "Unidad de peso").Value;
            _repository.GetByIdAsync(Arg.Any<UnitId>(), Arg.Any<CancellationToken>()).Returns(unit);

            var result = await CreateHandler().Handle(new GetUnitByIdQuery(_unitGuid), CancellationToken.None);

            Assert.False(result.IsError);
            Assert.Equal("Kilogramo", result.Value.Name);
        }
    }
}
