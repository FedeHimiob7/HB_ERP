using HB_ERP.SharedKernel.Domain.Primitives;
using Inventory.Application.ProductSubCategories.Commands.DeactivateProductSubCategory;
using Inventory.Domain.DomainErrors;
using Inventory.Domain.Entities;
using Inventory.Domain.Repositories;
using Inventory.Domain.VO;
using NSubstitute;
using Xunit;

namespace Application.Tests
{
    public sealed class DeactivateProductSubCategoryCommandHandlerTests
    {
        private readonly IProductSubCategoryRepository _repository = Substitute.For<IProductSubCategoryRepository>();
        private readonly IProductCategoryRepository _categoryRepository = Substitute.For<IProductCategoryRepository>();
        private readonly Inventory.Application.Interfaces.IInventoryUnitOfWork _unitOfWork = Substitute.For<Inventory.Application.Interfaces.IInventoryUnitOfWork>();
        private readonly ICurrentUserProvider _currentUser = Substitute.For<ICurrentUserProvider>();
        private readonly Guid _subCategoryGuid = Guid.NewGuid();

        private DeactivateProductSubCategoryCommandHandler CreateHandler()
            => new(_repository, _categoryRepository, _unitOfWork, _currentUser);

        [Fact]
        public async Task Handle_WhenSubCategoryDoesNotExist_ReturnsNotFound()
        {
            _repository.GetByIdAsync(Arg.Any<ProductSubCategoryId>(), Arg.Any<CancellationToken>()).Returns((ProductSubCategory?)null);

            var result = await CreateHandler().Handle(new DeactivateProductSubCategoryCommand(_subCategoryGuid), CancellationToken.None);

            Assert.True(result.IsError);
            Assert.Equal(ProductSubCategoryErrors.NotFound.Code, result.FirstError.Code);
        }

        [Fact]
        public async Task Handle_WhenUserHasNoAccessToParentPsl_ReturnsNotFound()
        {
            var subCategory = ProductSubCategory.CreateExisting(_subCategoryGuid, Guid.NewGuid(), "Sandalias", null, isActive: true);
            _repository.GetByIdAsync(Arg.Any<ProductSubCategoryId>(), Arg.Any<CancellationToken>()).Returns(subCategory);
            _categoryRepository.GetByIdAsync(Arg.Any<Inventory.Domain.VO.ProductCategoryId>(), Arg.Any<IReadOnlyList<Guid>>(), Arg.Any<CancellationToken>())
                .Returns((ProductCategory?)null);

            var result = await CreateHandler().Handle(new DeactivateProductSubCategoryCommand(_subCategoryGuid), CancellationToken.None);

            Assert.True(result.IsError);
            Assert.Equal(ProductSubCategoryErrors.NotFound.Code, result.FirstError.Code);
        }

        [Fact]
        public async Task Handle_WhenValid_DeactivatesAndPersists()
        {
            var subCategory = ProductSubCategory.CreateExisting(_subCategoryGuid, Guid.NewGuid(), "Sandalias", null, isActive: true);
            _repository.GetByIdAsync(Arg.Any<ProductSubCategoryId>(), Arg.Any<CancellationToken>()).Returns(subCategory);
            var category = ProductCategory.CreateExisting(Guid.NewGuid(), Guid.NewGuid(), null, "Calzado", null, isActive: true);
            _categoryRepository.GetByIdAsync(Arg.Any<Inventory.Domain.VO.ProductCategoryId>(), Arg.Any<IReadOnlyList<Guid>>(), Arg.Any<CancellationToken>())
                .Returns(category);

            var result = await CreateHandler().Handle(new DeactivateProductSubCategoryCommand(_subCategoryGuid), CancellationToken.None);

            Assert.False(result.IsError);
            Assert.False(subCategory.IsActive);
            await _repository.Received(1).UpdateAsync(subCategory, Arg.Any<CancellationToken>());
            await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }
    }
}
