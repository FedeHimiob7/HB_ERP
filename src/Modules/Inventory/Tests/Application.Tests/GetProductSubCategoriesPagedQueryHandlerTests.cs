using HB_ERP.SharedKernel.Domain.Primitives;
using Inventory.Application.ProductSubCategories.Queries.GetPaged;
using Inventory.Domain.Entities;
using Inventory.Domain.Repositories;
using Inventory.Domain.SearchParametersModel;
using NSubstitute;
using Xunit;

namespace Application.Tests
{
    public sealed class GetProductSubCategoriesPagedQueryHandlerTests
    {
        private readonly IProductSubCategoryRepository _repository = Substitute.For<IProductSubCategoryRepository>();
        private readonly ICurrentUserProvider _currentUser = Substitute.For<ICurrentUserProvider>();

        private GetProductSubCategoriesPagedQueryHandler CreateHandler() => new(_repository, _currentUser);

        [Fact]
        public async Task Handle_PassesAllowedPslIdsAndReturnsPagedResult()
        {
            var allowedIds = new List<Guid> { Guid.NewGuid() };
            _currentUser.PslIds.Returns(allowedIds);
            var items = new List<ProductSubCategory> { ProductSubCategory.CreateExisting(Guid.NewGuid(), Guid.NewGuid(), "Sandalias", null, isActive: true) };
            var filter = new ProductSubCategoryFilter(1, 10, "San");
            _repository.GetPagedAsync(filter, allowedIds, Arg.Any<CancellationToken>()).Returns((items, 1));

            var result = await CreateHandler().Handle(new GetProductSubCategoriesPagedQuery(filter), CancellationToken.None);

            Assert.False(result.IsError);
            Assert.Single(result.Value.Items);
            Assert.Equal(1, result.Value.TotalCount);
        }
    }
}
