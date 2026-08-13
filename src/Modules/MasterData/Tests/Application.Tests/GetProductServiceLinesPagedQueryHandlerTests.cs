using HB_ERP.SharedKernel.Domain.Primitives;
using MasterData.Application.ProductServiceLines.Queries.GetPaged;
using MasterData.Domain.Entities;
using MasterData.Domain.Repositories;
using NSubstitute;
using Xunit;

namespace Application.Tests
{
    public sealed class GetProductServiceLinesPagedQueryHandlerTests
    {
        private readonly IProductServiceLineRepository _repository = Substitute.For<IProductServiceLineRepository>();
        private readonly ICurrentUserProvider _currentUser = Substitute.For<ICurrentUserProvider>();

        private GetProductServiceLinesPagedQueryHandler CreateHandler() => new(_repository, _currentUser);

        [Fact]
        public async Task Handle_PassesAllowedPslIdsAndReturnsPagedResult()
        {
            var allowedIds = new List<Guid> { Guid.NewGuid() };
            _currentUser.PslIds.Returns(allowedIds);

            var lines = new List<ProductServiceLine> { ProductServiceLine.CreateExisting(allowedIds[0], "Desc", "Calzado", isActive: true) };
            _repository.GetPagedAsync(1, 10, allowedIds, "Cal", Arg.Any<CancellationToken>()).Returns((lines, 1));

            var result = await CreateHandler().Handle(new GetProductServiceLinesPagedQuery(1, 10, "Cal"), CancellationToken.None);

            Assert.False(result.IsError);
            Assert.Single(result.Value.Items);
            Assert.Equal(1, result.Value.TotalCount);
        }
    }
}
