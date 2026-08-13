using Inventory.Application.ProductBrands.Queries.GetById;
using Inventory.Domain.DomainErrors;
using Inventory.Domain.Entities;
using Inventory.Domain.Repositories;
using Inventory.Domain.VO;
using NSubstitute;
using Xunit;

namespace Application.Tests
{
    public sealed class GetProductBrandByIdQueryHandlerTests
    {
        private readonly IProductBrandRepository _repository = Substitute.For<IProductBrandRepository>();
        private readonly Guid _brandGuid = Guid.NewGuid();

        private GetProductBrandByIdQueryHandler CreateHandler() => new(_repository);

        [Fact]
        public async Task Handle_WhenNotFound_ReturnsNotFound()
        {
            _repository.GetByIdAsync(Arg.Any<ProductBrandId>(), Arg.Any<CancellationToken>()).Returns((ProductBrand?)null);

            var result = await CreateHandler().Handle(new GetProductBrandByIdQuery(_brandGuid), CancellationToken.None);

            Assert.True(result.IsError);
            Assert.Equal(ProductBrandErrors.NotFound.Code, result.FirstError.Code);
        }

        [Fact]
        public async Task Handle_WhenFound_ReturnsResponse()
        {
            var brand = ProductBrand.CreateExisting(_brandGuid, "Nike", null, isActive: true);
            _repository.GetByIdAsync(Arg.Any<ProductBrandId>(), Arg.Any<CancellationToken>()).Returns(brand);

            var result = await CreateHandler().Handle(new GetProductBrandByIdQuery(_brandGuid), CancellationToken.None);

            Assert.False(result.IsError);
            Assert.Equal("Nike", result.Value.Name);
        }
    }
}
