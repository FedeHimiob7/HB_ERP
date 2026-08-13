using HB_ERP.SharedKernel.Domain.Primitives;
using Inventory.Application.Products.Queries.GetPaged;
using Inventory.Domain.Entities;
using Inventory.Domain.Repositories;
using Inventory.Domain.SearchParametersModel;
using MasterData.Domain.VO;
using NSubstitute;
using Xunit;

namespace Application.Tests
{
    public sealed class GetProductsPagedQueryHandlerTests
    {
        private readonly IProductRepository _repository = Substitute.For<IProductRepository>();
        private readonly ICurrentUserProvider _currentUser = Substitute.For<ICurrentUserProvider>();

        private GetProductsPagedQueryHandler CreateHandler() => new(_repository, _currentUser);

        [Fact]
        public async Task Handle_PassesAllowedPslIdsAndReturnsPagedResult()
        {
            var allowedIds = new List<Guid> { Guid.NewGuid() };
            _currentUser.PslIds.Returns(allowedIds);
            var product = Product.Create(
                "20260810-1-1-1", 1, "Zapato deportivo", ProductServiceLineId.Create(allowedIds[0]),
                null, null, null, null, null, null, true, true, true).Value;
            var filter = new ProductFilter(1, 10, "Zapato");
            _repository.GetPagedAsync(filter, allowedIds, Arg.Any<CancellationToken>())
                .Returns((new List<Product> { product }, 1));

            var result = await CreateHandler().Handle(new GetProductsPagedQuery(filter), CancellationToken.None);

            Assert.False(result.IsError);
            Assert.Single(result.Value.Items);
            Assert.Equal(1, result.Value.TotalCount);
        }
    }
}
