using Inventory.Application.ProductBrands.Queries.GetAll;
using Inventory.Domain.Entities;
using Inventory.Domain.Repositories;
using NSubstitute;
using Xunit;

namespace Application.Tests
{
    public sealed class GetAllProductBrandsQueryHandlerTests
    {
        private readonly IProductBrandRepository _repository = Substitute.For<IProductBrandRepository>();

        private GetAllProductBrandsQueryHandler CreateHandler() => new(_repository);

        [Fact]
        public async Task Handle_MapsAllItemsToResponse()
        {
            var items = new List<ProductBrand> { ProductBrand.CreateExisting(Guid.NewGuid(), "Nike", null, isActive: true) };
            _repository.GetAllAsync(Arg.Any<CancellationToken>()).Returns(items);

            var result = await CreateHandler().Handle(new GetAllProductBrandsQuery(), CancellationToken.None);

            Assert.False(result.IsError);
            Assert.Single(result.Value);
            Assert.Equal("Nike", result.Value[0].Name);
        }
    }
}
