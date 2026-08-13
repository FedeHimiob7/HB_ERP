using Identity.Application.Roles.Queries.GetRolePagedQuery;
using Identity.Domain.Entities;
using Identity.Domain.Repositories;
using NSubstitute;

namespace Application.Tests
{
    public sealed class GetRolesPagedQueryHandlerTests
    {
        private readonly IRoleRepository _repository = Substitute.For<IRoleRepository>();

        private GetRolesPagedQueryHandler CreateHandler() => new(_repository);

        [Fact]
        public async Task Handle_ReturnsPagedListWithActionIdsMapped()
        {
            var role = Role.Create("Ventas");
            role.AssignAction(new Identity.Domain.VO.ActionsId(Guid.NewGuid()));
            _repository.GetPagedAsync(1, 10, Arg.Any<CancellationToken>()).Returns((new List<Role> { role }, 7));

            var result = await CreateHandler().Handle(new GetRolesPagedQuery(1, 10), CancellationToken.None);

            Assert.False(result.IsError);
            Assert.Single(result.Value.Items);
            Assert.Equal(7, result.Value.TotalCount);
            Assert.Single(result.Value.Items[0].Actions);
        }
    }
}
