using MasterData.Application.Interfaces;
using MasterData.Application.Units.Commands.DeleteUnit;
using MasterData.Domain.DomainErrors;
using MasterData.Domain.Entities;
using MasterData.Domain.Repositories;
using MasterData.Domain.VO;
using NSubstitute;
using Xunit;

namespace Application.Tests
{
    public sealed class DeactivateUnitCommandHandlerTests
    {
        private readonly IUnitRepository _repository = Substitute.For<IUnitRepository>();
        private readonly IMasterDataUnitOfWork _unitOfWork = Substitute.For<IMasterDataUnitOfWork>();
        private readonly Guid _unitGuid = Guid.NewGuid();

        private DeactivateUnitCommandHandler CreateHandler() => new(_repository, _unitOfWork);

        [Fact]
        public async Task Handle_WhenUnitDoesNotExist_ReturnsNotFound()
        {
            _repository.GetByIdAsync(Arg.Any<UnitId>(), Arg.Any<CancellationToken>()).Returns((Unit?)null);

            var result = await CreateHandler().Handle(new DeactivateUnitCommand(_unitGuid), CancellationToken.None);

            Assert.True(result.IsError);
            Assert.Equal(UnitErrors.NotFound.Code, result.FirstError.Code);
        }

        [Fact]
        public async Task Handle_WhenValid_DeactivatesAndPersists()
        {
            var unit = Unit.Create("Kilogramo", "Unidad de peso").Value;
            _repository.GetByIdAsync(Arg.Any<UnitId>(), Arg.Any<CancellationToken>()).Returns(unit);

            var result = await CreateHandler().Handle(new DeactivateUnitCommand(_unitGuid), CancellationToken.None);

            Assert.False(result.IsError);
            Assert.False(unit.IsActive);

            await _repository.Received(1).UpdateAsync(unit, Arg.Any<CancellationToken>());
            await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }
    }
}
