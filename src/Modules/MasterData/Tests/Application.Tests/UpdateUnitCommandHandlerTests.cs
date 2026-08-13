using MasterData.Application.Interfaces;
using MasterData.Application.Units.Commands.UpdateUnit;
using MasterData.Domain.DomainErrors;
using MasterData.Domain.Entities;
using MasterData.Domain.Repositories;
using MasterData.Domain.VO;
using NSubstitute;
using Xunit;

namespace Application.Tests
{
    public sealed class UpdateUnitCommandHandlerTests
    {
        private readonly IUnitRepository _repository = Substitute.For<IUnitRepository>();
        private readonly IMasterDataUnitOfWork _unitOfWork = Substitute.For<IMasterDataUnitOfWork>();
        private readonly Guid _unitGuid = Guid.NewGuid();

        private UpdateUnitCommandHandler CreateHandler() => new(_repository, _unitOfWork);

        [Fact]
        public async Task Handle_WhenUnitDoesNotExist_ReturnsNotFound()
        {
            _repository.GetByIdAsync(Arg.Any<UnitId>(), Arg.Any<CancellationToken>()).Returns((Unit?)null);

            var command = new UpdateUnitCommand(_unitGuid, "Kilogramo", "Unidad de peso");
            var result = await CreateHandler().Handle(command, CancellationToken.None);

            Assert.True(result.IsError);
            Assert.Equal(UnitErrors.NotFound.Code, result.FirstError.Code);
        }

        [Fact]
        public async Task Handle_WhenValid_UpdatesAndReturnsResponse()
        {
            // Unit no tiene CreateExisting — se reconstruye vía Create() normal, alcanza para el test.
            var unit = Unit.Create("Gramo", "Unidad de peso chica").Value;
            _repository.GetByIdAsync(Arg.Any<UnitId>(), Arg.Any<CancellationToken>()).Returns(unit);

            var command = new UpdateUnitCommand(_unitGuid, "Kilogramo", "Unidad de peso");
            var result = await CreateHandler().Handle(command, CancellationToken.None);

            Assert.False(result.IsError);
            Assert.Equal("Kilogramo", result.Value.Name);
            Assert.Equal("Unidad de peso", result.Value.Description);

            await _repository.Received(1).UpdateAsync(unit, Arg.Any<CancellationToken>());
            await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }
    }
}
