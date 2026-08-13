using Identity.Application.SystemActions.Queries.GetAllSystemActions;
using Identity.Domain.Entities;
using Identity.Domain.Repositories;
using NSubstitute;

namespace Application.Tests
{
    public sealed class GetAllSystemActionsQueryHandlerTests
    {
        private readonly ISystemActionRepository _repository = Substitute.For<ISystemActionRepository>();

        private GetAllSystemActionsQueryHandler CreateHandler() => new(_repository);

        [Fact]
        public async Task Handle_MapsAllActionsToResponse()
        {
            var actions = new List<SystemAction> { SystemAction.Create("products.create", "Crear productos") };
            _repository.GetAllAsync().Returns(actions);

            var result = await CreateHandler().Handle(new GetAllSystemActionsQuery(), CancellationToken.None);

            Assert.False(result.IsError);
            Assert.Single(result.Value);
            Assert.Equal("products.create", result.Value[0].Name);
        }
    }
}
