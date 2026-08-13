using HB_ERP.SharedKernel.Domain.Primitives;
using Inventory.Application.Products.Queries.GetById;
using Inventory.Domain.DomainErrors;
using Inventory.Domain.Entities;
using Inventory.Domain.Repositories;
using Inventory.Domain.VO;
using MasterData.Domain.VO;
using NSubstitute;
using Xunit;

namespace Application.Tests
{
    public sealed class GetProductByIdQueryHandlerTests
    {
        private readonly IProductRepository _repository = Substitute.For<IProductRepository>();
        private readonly ICurrentUserProvider _currentUser = Substitute.For<ICurrentUserProvider>();
        private readonly Guid _productGuid = Guid.NewGuid();

        private GetProductByIdQueryHandler CreateHandler() => new(_repository, _currentUser);

        [Fact]
        public async Task Handle_WhenNotFoundOrNoPslAccess_ReturnsNotFound()
        {
            _repository.GetByIdAsync(Arg.Any<ProductId>(), Arg.Any<IReadOnlyList<Guid>>(), Arg.Any<CancellationToken>())
                .Returns((Product?)null);

            var result = await CreateHandler().Handle(new GetProductByIdQuery(_productGuid), CancellationToken.None);

            Assert.True(result.IsError);
            Assert.Equal(ProductErrors.NotFound.Code, result.FirstError.Code);
        }

        [Fact]
        public async Task Handle_WhenFound_ReturnsResponse()
        {
            var product = Product.Create(
                "20260810-1-1-1", 1, "Zapato deportivo", ProductServiceLineId.New(),
                null, null, null, null, null, null, true, true, true).Value;
            _repository.GetByIdAsync(Arg.Any<ProductId>(), Arg.Any<IReadOnlyList<Guid>>(), Arg.Any<CancellationToken>()).Returns(product);

            var result = await CreateHandler().Handle(new GetProductByIdQuery(_productGuid), CancellationToken.None);

            Assert.False(result.IsError);
            Assert.Equal("Zapato deportivo", result.Value.Name);
        }
    }
}
