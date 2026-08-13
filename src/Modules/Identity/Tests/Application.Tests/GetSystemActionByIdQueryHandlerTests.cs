using Identity.Application.SystemActions.Queries.GetSystemActionById;
using Identity.Domain.DomainErrors;
using Identity.Domain.Entities;
using Identity.Domain.Repositories;
using Identity.Domain.VO;
using NSubstitute;

namespace Application.Tests
{
    public sealed class GetSystemActionByIdQueryHandlerTests
    {
        private readonly ISystemActionRepository _repository = Substitute.For<ISystemActionRepository>();
        private readonly Guid _actionGuid = Guid.NewGuid();

        private GetSystemActionByIdQueryHandler CreateHandler() => new(_repository);

        [Fact]
        public async Task Handle_WhenNotFound_ReturnsNotFound()
        {
            _repository.GetByIdAsync(Arg.Any<ActionsId>()).Returns((SystemAction?)null);

            var result = await CreateHandler().Handle(new GetSystemActionByIdQuery(_actionGuid), CancellationToken.None);

            Assert.True(result.IsError);
            Assert.Equal(SystemActionErrors.NotFound.Code, result.FirstError.Code);
        }

        [Fact]
        public async Task Handle_WhenFound_ReturnsResponse()
        {
            var action = SystemAction.Create("products.create", "Crear productos");
            _repository.GetByIdAsync(Arg.Any<ActionsId>()).Returns(action);

            var result = await CreateHandler().Handle(new GetSystemActionByIdQuery(_actionGuid), CancellationToken.None);

            Assert.False(result.IsError);
            Assert.Equal("products.create", result.Value.Name);
        }
    }
}
