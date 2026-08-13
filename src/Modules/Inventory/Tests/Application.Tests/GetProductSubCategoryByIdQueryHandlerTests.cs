using HB_ERP.SharedKernel.Domain.Primitives;
using Inventory.Application.ProductSubCategories.Queries.GetById;
using Inventory.Domain.DomainErrors;
using Inventory.Domain.Entities;
using Inventory.Domain.Repositories;
using Inventory.Domain.VO;
using NSubstitute;
using Xunit;

namespace Application.Tests
{
    public sealed class GetProductSubCategoryByIdQueryHandlerTests
    {
        private readonly IProductSubCategoryRepository _repository = Substitute.For<IProductSubCategoryRepository>();
        private readonly IProductCategoryRepository _categoryRepository = Substitute.For<IProductCategoryRepository>();
        private readonly ICurrentUserProvider _currentUser = Substitute.For<ICurrentUserProvider>();
        private readonly Guid _subCategoryGuid = Guid.NewGuid();

        private GetProductSubCategoryByIdQueryHandler CreateHandler() => new(_repository, _categoryRepository, _currentUser);

        [Fact]
        public async Task Handle_WhenNotFound_ReturnsNotFound()
        {
            _repository.GetByIdAsync(Arg.Any<ProductSubCategoryId>(), Arg.Any<CancellationToken>()).Returns((ProductSubCategory?)null);

            var result = await CreateHandler().Handle(new GetProductSubCategoryByIdQuery(_subCategoryGuid), CancellationToken.None);

            Assert.True(result.IsError);
            Assert.Equal(ProductSubCategoryErrors.NotFound.Code, result.FirstError.Code);
        }

        [Fact]
        public async Task Handle_WhenFoundAndUserHasAccess_ReturnsResponse()
        {
            var subCategory = ProductSubCategory.CreateExisting(_subCategoryGuid, Guid.NewGuid(), "Sandalias", null, isActive: true);
            _repository.GetByIdAsync(Arg.Any<ProductSubCategoryId>(), Arg.Any<CancellationToken>()).Returns(subCategory);
            var category = ProductCategory.CreateExisting(Guid.NewGuid(), Guid.NewGuid(), null, "Calzado", null, isActive: true);
            _categoryRepository.GetByIdAsync(Arg.Any<Inventory.Domain.VO.ProductCategoryId>(), Arg.Any<IReadOnlyList<Guid>>(), Arg.Any<CancellationToken>())
                .Returns(category);

            var result = await CreateHandler().Handle(new GetProductSubCategoryByIdQuery(_subCategoryGuid), CancellationToken.None);

            Assert.False(result.IsError);
            Assert.Equal("Sandalias", result.Value.Name);
        }
    }
}
