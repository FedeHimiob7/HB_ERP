using MasterData.Application.States.Queries.GetById;
using MasterData.Domain.DomainErrors;
using MasterData.Domain.Entities;
using MasterData.Domain.Repositories;
using MasterData.Domain.VO;
using NSubstitute;
using Xunit;

namespace Application.Tests
{
    public sealed class GetStateByIdQueryHandlerTests
    {
        private readonly IStateRepository _repository = Substitute.For<IStateRepository>();
        private readonly Guid _stateGuid = Guid.NewGuid();

        private GetStateByIdQueryHandler CreateHandler() => new(_repository);

        [Fact]
        public async Task Handle_WhenNotFound_ReturnsNotFound()
        {
            _repository.GetByIdAsync(Arg.Any<StateId>(), Arg.Any<CancellationToken>()).Returns((State?)null);

            var result = await CreateHandler().Handle(new GetStateByIdQuery(_stateGuid), CancellationToken.None);

            Assert.True(result.IsError);
            Assert.Equal(StateErrors.NotFound.Code, result.FirstError.Code);
        }

        [Fact]
        public async Task Handle_WhenFound_ReturnsResponse()
        {
            var state = State.CreateExisting(_stateGuid, Guid.NewGuid(), "MI", "Miranda", isActive: true);
            _repository.GetByIdAsync(Arg.Any<StateId>(), Arg.Any<CancellationToken>()).Returns(state);

            var result = await CreateHandler().Handle(new GetStateByIdQuery(_stateGuid), CancellationToken.None);

            Assert.False(result.IsError);
            Assert.Equal(_stateGuid, result.Value.Id);
        }
    }
}
