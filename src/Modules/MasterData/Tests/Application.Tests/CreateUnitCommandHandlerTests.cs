using MasterData.Application.Interfaces;
using MasterData.Application.Units.Commands.CreateUnit;
using MasterData.Domain.DomainErrors;
using MasterData.Domain.Repositories;
using NSubstitute;
using Xunit;

namespace Application.Tests
{
    public sealed class CreateUnitCommandHandlerTests
    {
        private readonly IUnitRepository _repository = Substitute.For<IUnitRepository>();
        private readonly IMasterDataUnitOfWork _unitOfWork = Substitute.For<IMasterDataUnitOfWork>();

        private CreateUnitCommandHandler CreateHandler() => new(_repository, _unitOfWork);

        [Fact]
        public async Task Handle_WhenDescriptionIsEmpty_ReturnsDescriptionIsRequired()
        {
            var result = await CreateHandler().Handle(new CreateUnitCommand("Kilogramo", ""), CancellationToken.None);

            Assert.True(result.IsError);
            Assert.Equal(UnitErrors.DescriptionIsRequired.Code, result.FirstError.Code);
        }

        [Fact]
        public async Task Handle_WhenValid_PersistsUnitAndReturnsId()
        {
            var result = await CreateHandler().Handle(new CreateUnitCommand("Kilogramo", "Unidad de peso"), CancellationToken.None);

            Assert.False(result.IsError);
            Assert.NotEqual(Guid.Empty, result.Value);

            await _repository.Received(1).AddAsync(Arg.Any<MasterData.Domain.Entities.Unit>(), Arg.Any<CancellationToken>());
            await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }
    }
}
