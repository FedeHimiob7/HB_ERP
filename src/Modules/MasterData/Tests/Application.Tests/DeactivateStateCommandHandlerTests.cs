using MasterData.Application.Interfaces;
using MasterData.Application.States.Commands.DeleteState;
using MasterData.Domain.DomainErrors;
using MasterData.Domain.Entities;
using MasterData.Domain.Repositories;
using MasterData.Domain.VO;
using NSubstitute;
using Xunit;

namespace Application.Tests
{
    public sealed class DeactivateStateCommandHandlerTests
    {
        private readonly IStateRepository _repository = Substitute.For<IStateRepository>();
        private readonly IMasterDataUnitOfWork _unitOfWork = Substitute.For<IMasterDataUnitOfWork>();
        private readonly Guid _stateGuid = Guid.NewGuid();

        private DeactivateStateCommandHandler CreateHandler() => new(_repository, _unitOfWork);

        [Fact]
        public async Task Handle_WhenStateDoesNotExist_ReturnsNotFound()
        {
            _repository.GetByIdAsync(Arg.Any<StateId>(), Arg.Any<CancellationToken>()).Returns((State?)null);

            var result = await CreateHandler().Handle(new DeactivateStateCommand(_stateGuid), CancellationToken.None);

            Assert.True(result.IsError);
            Assert.Equal(StateErrors.NotFound.Code, result.FirstError.Code);
        }

        [Fact]
        public async Task Handle_WhenValid_DeactivatesAndPersists()
        {
            var state = State.CreateExisting(_stateGuid, Guid.NewGuid(), "MI", "Miranda", isActive: true);
            _repository.GetByIdAsync(Arg.Any<StateId>(), Arg.Any<CancellationToken>()).Returns(state);

            var result = await CreateHandler().Handle(new DeactivateStateCommand(_stateGuid), CancellationToken.None);

            Assert.False(result.IsError);
            Assert.False(state.IsActive);

            await _repository.Received(1).UpdateAsync(state, Arg.Any<CancellationToken>());
            await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }
    }
}
