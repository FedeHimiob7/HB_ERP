using Identity.Application.Users.Queries.GetUserById;
using Identity.Domain;
using Identity.Domain.DomainErrors;
using Identity.Domain.VO;
using NSubstitute;

namespace Application.Tests
{
    public sealed class GetUserByIdQueryHandlerTests
    {
        private readonly IUserRepository _repository = Substitute.For<IUserRepository>();
        private readonly Guid _userGuid = Guid.NewGuid();

        private GetUserByIdQueryHandler CreateHandler() => new(_repository);

        [Fact]
        public async Task Handle_WhenNotFound_ReturnsNotFound()
        {
            _repository.GetByIdAsync(Arg.Any<UserId>()).Returns((User?)null);

            var result = await CreateHandler().Handle(new GetUserByIdQuery(_userGuid), CancellationToken.None);

            Assert.True(result.IsError);
            Assert.Equal(UserErrors.NotFound.Code, result.FirstError.Code);
        }

        [Fact]
        public async Task Handle_WhenFound_ReturnsResponse()
        {
            var user = User.Register("Juan", "Perez", Email.Create("juan@ejemplo.com").Value, PasswordHash.Create("hash"));
            _repository.GetByIdAsync(Arg.Any<UserId>()).Returns(user);

            var result = await CreateHandler().Handle(new GetUserByIdQuery(_userGuid), CancellationToken.None);

            Assert.False(result.IsError);
            Assert.Equal("juan@ejemplo.com", result.Value.Email);
        }
    }
}
