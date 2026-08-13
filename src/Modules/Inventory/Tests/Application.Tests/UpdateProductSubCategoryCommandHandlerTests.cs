using HB_ERP.SharedKernel.Domain.Primitives;
using Inventory.Application.ProductSubCategories.Commands.UpdateProductSubCategory;
using Inventory.Domain.DomainErrors;
using Inventory.Domain.Entities;
using Inventory.Domain.Repositories;
using Inventory.Domain.VO;
using NSubstitute;
using Xunit;

namespace Application.Tests
{
    public sealed class UpdateProductSubCategoryCommandHandlerTests
    {
        private readonly IProductSubCategoryRepository _repository = Substitute.For<IProductSubCategoryRepository>();
        private readonly IProductCategoryRepository _categoryRepository = Substitute.For<IProductCategoryRepository>();
        private readonly Inventory.Application.Interfaces.IInventoryUnitOfWork _unitOfWork = Substitute.For<Inventory.Application.Interfaces.IInventoryUnitOfWork>();
        private readonly ICurrentUserProvider _currentUser = Substitute.For<ICurrentUserProvider>();
        private readonly Guid _subCategoryGuid = Guid.NewGuid();
        private readonly Guid _categoryGuid = Guid.NewGuid();

        private UpdateProductSubCategoryCommandHandler CreateHandler()
            => new(_repository, _categoryRepository, _unitOfWork, _currentUser);

        [Fact]
        public async Task Handle_WhenSubCategoryDoesNotExist_ReturnsNotFound()
        {
            _repository.GetByIdAsync(Arg.Any<ProductSubCategoryId>(), Arg.Any<CancellationToken>()).Returns((ProductSubCategory?)null);

            var command = new UpdateProductSubCategoryCommand(_subCategoryGuid, _categoryGuid, "Zapatillas", null);
            var result = await CreateHandler().Handle(command, CancellationToken.None);

            Assert.True(result.IsError);
            Assert.Equal(ProductSubCategoryErrors.NotFound.Code, result.FirstError.Code);
        }

        [Fact]
        public async Task Handle_WhenValid_UpdatesAndReturnsResponse()
        {
            var subCategory = ProductSubCategory.CreateExisting(_subCategoryGuid, _categoryGuid, "Sandalias", null, isActive: true);
            _repository.GetByIdAsync(Arg.Any<ProductSubCategoryId>(), Arg.Any<CancellationToken>()).Returns(subCategory);
            var category = ProductCategory.CreateExisting(_categoryGuid, Guid.NewGuid(), null, "Calzado", null, isActive: true);
            _categoryRepository.GetByIdAsync(Arg.Any<ProductCategoryId>(), Arg.Any<IReadOnlyList<Guid>>(), Arg.Any<CancellationToken>()).Returns(category);
            _repository.ExistsByNameInCategoryAsync(Arg.Any<string>(), Arg.Any<ProductCategoryId>(), excludeId: Arg.Any<ProductSubCategoryId?>(), cancellationToken: Arg.Any<CancellationToken>())
                .Returns(false);

            var command = new UpdateProductSubCategoryCommand(_subCategoryGuid, _categoryGuid, "Zapatillas", null);
            var result = await CreateHandler().Handle(command, CancellationToken.None);

            Assert.False(result.IsError);
            Assert.Equal("Zapatillas", result.Value.Name);
            await _repository.Received(1).UpdateAsync(subCategory, Arg.Any<CancellationToken>());
            await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }
    }
}
