using Identity.Application.Users.Queries.GetUsersPaged;
using Identity.Domain;
using Identity.Domain.VO;
using NSubstitute;

namespace Application.Tests
{
    public sealed class GetUsersPagedQueryHandlerTests
    {
        private readonly IUserRepository _repository = Substitute.For<IUserRepository>();

        private GetUsersPagedQueryHandler CreateHandler() => new(_repository);

        [Fact]
        public async Task Handle_ReturnsPagedListWithTotalCount()
        {
            var user = User.Register("Juan", "Perez", Email.Create("juan@ejemplo.com").Value, PasswordHash.Create("hash"));
            _repository.GetPagedAsync(1, 10, Arg.Any<CancellationToken>()).Returns((new List<User> { user }, 30));

            var result = await CreateHandler().Handle(new GetUsersPagedQuery(1, 10), CancellationToken.None);

            Assert.False(result.IsError);
            Assert.Single(result.Value.Items);
            Assert.Equal(30, result.Value.TotalCount);
        }
    }
}
