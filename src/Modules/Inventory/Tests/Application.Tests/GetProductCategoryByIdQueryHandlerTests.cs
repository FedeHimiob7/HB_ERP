using HB_ERP.SharedKernel.Domain.Primitives;
using Inventory.Application.ProductCategories.Queries.GetById;
using Inventory.Domain.DomainErrors;
using Inventory.Domain.Entities;
using Inventory.Domain.Repositories;
using Inventory.Domain.VO;
using NSubstitute;
using Xunit;

namespace Application.Tests
{
    public sealed class GetProductCategoryByIdQueryHandlerTests
    {
        private readonly IProductCategoryRepository _repository = Substitute.For<IProductCategoryRepository>();
        private readonly ICurrentUserProvider _currentUser = Substitute.For<ICurrentUserProvider>();
        private readonly Guid _categoryGuid = Guid.NewGuid();

        private GetProductCategoryByIdQueryHandler CreateHandler() => new(_repository, _currentUser);

        [Fact]
        public async Task Handle_WhenNotFound_ReturnsNotFound()
        {
            _repository.GetByIdAsync(Arg.Any<ProductCategoryId>(), Arg.Any<IReadOnlyList<Guid>>(), Arg.Any<CancellationToken>())
                .Returns((ProductCategory?)null);

            var result = await CreateHandler().Handle(new GetProductCategoryByIdQuery(_categoryGuid), CancellationToken.None);

            Assert.True(result.IsError);
            Assert.Equal(ProductCategoryErrors.NotFound.Code, result.FirstError.Code);
        }

        [Fact]
        public async Task Handle_WhenFound_ReturnsResponse()
        {
            var category = ProductCategory.CreateExisting(_categoryGuid, Guid.NewGuid(), null, "Calzado", null, isActive: true);
            _repository.GetByIdAsync(Arg.Any<ProductCategoryId>(), Arg.Any<IReadOnlyList<Guid>>(), Arg.Any<CancellationToken>()).Returns(category);

            var result = await CreateHandler().Handle(new GetProductCategoryByIdQuery(_categoryGuid), CancellationToken.None);

            Assert.False(result.IsError);
            Assert.Equal("Calzado", result.Value.Name);
        }
    }
}
