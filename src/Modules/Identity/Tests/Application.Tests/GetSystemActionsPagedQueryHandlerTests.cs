using Identity.Application.SystemActions.Queries.GetSystemActionsPaged;
using Identity.Domain.Entities;
using Identity.Domain.Repositories;
using NSubstitute;

namespace Application.Tests
{
    public sealed class GetSystemActionsPagedQueryHandlerTests
    {
        private readonly ISystemActionRepository _repository = Substitute.For<ISystemActionRepository>();

        private GetSystemActionsPagedQueryHandler CreateHandler() => new(_repository);

        [Fact]
        public async Task Handle_ReturnsPagedListWithTotalCount()
        {
            var actions = new List<SystemAction> { SystemAction.Create("products.create", "Crear productos") };
            _repository.GetPagedAsync(1, 10, Arg.Any<CancellationToken>()).Returns((actions, 12));

            var result = await CreateHandler().Handle(new GetSystemActionsPagedQuery(1, 10), CancellationToken.None);

            Assert.False(result.IsError);
            Assert.Single(result.Value.Items);
            Assert.Equal(12, result.Value.TotalCount);
        }
    }
}
