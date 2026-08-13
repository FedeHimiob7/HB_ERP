using Inventory.Application.ProductTypes.Queries.GetAll;
using Inventory.Domain.Entities;
using Inventory.Domain.Repositories;
using NSubstitute;
using Xunit;

namespace Application.Tests
{
    public sealed class GetAllProductTypesQueryHandlerTests
    {
        private readonly IProductTypeRepository _repository = Substitute.For<IProductTypeRepository>();

        private GetAllProductTypesQueryHandler CreateHandler() => new(_repository);

        [Fact]
        public async Task Handle_MapsAllItemsToResponse()
        {
            var items = new List<ProductType> { ProductType.CreateExisting(Guid.NewGuid(), "Bien", null, isActive: true) };
            _repository.GetAllAsync(Arg.Any<CancellationToken>()).Returns(items);

            var result = await CreateHandler().Handle(new GetAllProductTypesQuery(), CancellationToken.None);

            Assert.False(result.IsError);
            Assert.Single(result.Value);
            Assert.Equal("Bien", result.Value[0].Name);
        }
    }
}
