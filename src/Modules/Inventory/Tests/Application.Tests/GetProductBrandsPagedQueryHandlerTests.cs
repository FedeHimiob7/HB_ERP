using Inventory.Application.ProductBrands.Queries.GetPaged;
using Inventory.Domain.Entities;
using Inventory.Domain.Repositories;
using Inventory.Domain.SearchParametersModel;
using NSubstitute;
using Xunit;

namespace Application.Tests
{
    public sealed class GetProductBrandsPagedQueryHandlerTests
    {
        private readonly IProductBrandRepository _repository = Substitute.For<IProductBrandRepository>();

        private GetProductBrandsPagedQueryHandler CreateHandler() => new(_repository);

        [Fact]
        public async Task Handle_ReturnsPagedResultUsingFilterPassedByCaller()
        {
            var items = new List<ProductBrand> { ProductBrand.CreateExisting(Guid.NewGuid(), "Nike", null, isActive: true) };
            var filter = new ProductBrandFilter(1, 10, "Nike");
            _repository.GetPagedAsync(filter, Arg.Any<CancellationToken>()).Returns((items, 4));

            var result = await CreateHandler().Handle(new GetProductBrandsPagedQuery(filter), CancellationToken.None);

            Assert.False(result.IsError);
            Assert.Single(result.Value.Items);
            Assert.Equal(4, result.Value.TotalCount);
        }
    }
}
