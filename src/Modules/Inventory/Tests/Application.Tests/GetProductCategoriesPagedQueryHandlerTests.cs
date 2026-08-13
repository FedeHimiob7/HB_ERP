using HB_ERP.SharedKernel.Domain.Primitives;
using Inventory.Application.ProductCategories.Queries.GetPaged;
using Inventory.Domain.Entities;
using Inventory.Domain.Repositories;
using Inventory.Domain.SearchParametersModel;
using NSubstitute;
using Xunit;

namespace Application.Tests
{
    public sealed class GetProductCategoriesPagedQueryHandlerTests
    {
        private readonly IProductCategoryRepository _repository = Substitute.For<IProductCategoryRepository>();
        private readonly ICurrentUserProvider _currentUser = Substitute.For<ICurrentUserProvider>();

        private GetProductCategoriesPagedQueryHandler CreateHandler() => new(_repository, _currentUser);

        [Fact]
        public async Task Handle_PassesAllowedPslIdsAndReturnsPagedResult()
        {
            var allowedIds = new List<Guid> { Guid.NewGuid() };
            _currentUser.PslIds.Returns(allowedIds);
            var items = new List<ProductCategory> { ProductCategory.CreateExisting(Guid.NewGuid(), allowedIds[0], null, "Calzado", null, isActive: true) };
            var filter = new ProductCategoryFilter(1, 10, "Cal");
            _repository.GetPagedAsync(filter, allowedIds, Arg.Any<CancellationToken>()).Returns((items, 1));

            var result = await CreateHandler().Handle(new GetProductCategoriesPagedQuery(filter), CancellationToken.None);

            Assert.False(result.IsError);
            Assert.Single(result.Value.Items);
            Assert.Equal(1, result.Value.TotalCount);
        }
    }
}
