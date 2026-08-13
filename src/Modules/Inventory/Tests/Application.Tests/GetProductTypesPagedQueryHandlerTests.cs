using Inventory.Application.ProductTypes.Queries.GetPaged;
using Inventory.Domain.Entities;
using Inventory.Domain.Repositories;
using Inventory.Domain.SearchParametersModel;
using NSubstitute;
using Xunit;

namespace Application.Tests
{
    public sealed class GetProductTypesPagedQueryHandlerTests
    {
        private readonly IProductTypeRepository _repository = Substitute.For<IProductTypeRepository>();

        private GetProductTypesPagedQueryHandler CreateHandler() => new(_repository);

        [Fact]
        public async Task Handle_ReturnsPagedResultUsingFilterPassedByCaller()
        {
            var items = new List<ProductType> { ProductType.CreateExisting(Guid.NewGuid(), "Bien", null, isActive: true) };
            var filter = new ProductTypeFilter(1, 10, "Bien");
            _repository.GetPagedAsync(filter, Arg.Any<CancellationToken>()).Returns((items, 8));

            var result = await CreateHandler().Handle(new GetProductTypesPagedQuery(filter), CancellationToken.None);

            Assert.False(result.IsError);
            Assert.Single(result.Value.Items);
            Assert.Equal(8, result.Value.TotalCount);
        }
    }
}
