using Identity.Application.Roles.Queries.GetAllRoles;
using Identity.Domain.Entities;
using Identity.Domain.Repositories;
using NSubstitute;

namespace Application.Tests
{
    public sealed class GetAllRolesQueryHandlerTests
    {
        private readonly IRoleRepository _repository = Substitute.For<IRoleRepository>();

        private GetAllRolesQueryHandler CreateHandler() => new(_repository);

        [Fact]
        public async Task Handle_MapsAllRolesToSummaryResponse()
        {
            var roles = new List<Role> { Role.Create("Ventas") };
            _repository.GetAllAsync().Returns(roles);

            var result = await CreateHandler().Handle(new GetAllRolesQuery(), CancellationToken.None);

            Assert.False(result.IsError);
            Assert.Single(result.Value);
            Assert.Equal("Ventas", result.Value[0].Name);
        }
    }
}
