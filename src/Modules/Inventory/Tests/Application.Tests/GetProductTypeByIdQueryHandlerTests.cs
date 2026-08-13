using Inventory.Application.ProductTypes.Queries.GetById;
using Inventory.Domain.DomainErrors;
using Inventory.Domain.Entities;
using Inventory.Domain.Repositories;
using Inventory.Domain.VO;
using NSubstitute;
using Xunit;

namespace Application.Tests
{
    public sealed class GetProductTypeByIdQueryHandlerTests
    {
        private readonly IProductTypeRepository _repository = Substitute.For<IProductTypeRepository>();
        private readonly Guid _typeGuid = Guid.NewGuid();

        private GetProductTypeByIdQueryHandler CreateHandler() => new(_repository);

        [Fact]
        public async Task Handle_WhenNotFound_ReturnsNotFound()
        {
            _repository.GetByIdAsync(Arg.Any<ProductTypeId>(), Arg.Any<CancellationToken>()).Returns((ProductType?)null);

            var result = await CreateHandler().Handle(new GetProductTypeByIdQuery(_typeGuid), CancellationToken.None);

            Assert.True(result.IsError);
            Assert.Equal(ProductTypeErrors.NotFound.Code, result.FirstError.Code);
        }

        [Fact]
        public async Task Handle_WhenFound_ReturnsResponse()
        {
            var productType = ProductType.CreateExisting(_typeGuid, "Bien", null, isActive: true);
            _repository.GetByIdAsync(Arg.Any<ProductTypeId>(), Arg.Any<CancellationToken>()).Returns(productType);

            var result = await CreateHandler().Handle(new GetProductTypeByIdQuery(_typeGuid), CancellationToken.None);

            Assert.False(result.IsError);
            Assert.Equal("Bien", result.Value.Name);
        }
    }
}
