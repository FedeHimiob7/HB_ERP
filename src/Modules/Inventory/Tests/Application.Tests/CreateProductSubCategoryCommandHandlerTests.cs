using HB_ERP.SharedKernel.Domain.Primitives;
using Inventory.Application.ProductSubCategories.Commands.CreateProductSubCategory;
using Inventory.Domain.DomainErrors;
using Inventory.Domain.Entities;
using Inventory.Domain.Repositories;
using Inventory.Domain.VO;
using NSubstitute;
using Xunit;

namespace Application.Tests
{
    public sealed class CreateProductSubCategoryCommandHandlerTests
    {
        private readonly IProductSubCategoryRepository _repository = Substitute.For<IProductSubCategoryRepository>();
        private readonly IProductCategoryRepository _categoryRepository = Substitute.For<IProductCategoryRepository>();
        private readonly Inventory.Application.Interfaces.IInventoryUnitOfWork _unitOfWork = Substitute.For<Inventory.Application.Interfaces.IInventoryUnitOfWork>();
        private readonly ICurrentUserProvider _currentUser = Substitute.For<ICurrentUserProvider>();
        private readonly Guid _categoryGuid = Guid.NewGuid();

        private CreateProductSubCategoryCommandHandler CreateHandler()
            => new(_repository, _categoryRepository, _unitOfWork, _currentUser);

        [Fact]
        public async Task Handle_WhenCategoryDoesNotExistOrNoPslAccess_ReturnsInvalidCategory()
        {
            // Un solo query resuelve existencia Y acceso PSL indirecto (vía padre) — patrón
            // "PSL indirecto se resuelve via padre" ya establecido en el repo.
            _categoryRepository.GetByIdAsync(Arg.Any<ProductCategoryId>(), Arg.Any<IReadOnlyList<Guid>>(), Arg.Any<CancellationToken>())
                .Returns((ProductCategory?)null);

            var command = new CreateProductSubCategoryCommand(_categoryGuid, "Zapatillas", null);
            var result = await CreateHandler().Handle(command, CancellationToken.None);

            Assert.True(result.IsError);
            Assert.Equal(ProductSubCategoryErrors.InvalidCategory.Code, result.FirstError.Code);
        }

        [Fact]
        public async Task Handle_WhenValid_PersistsAndReturnsId()
        {
            var category = ProductCategory.CreateExisting(_categoryGuid, Guid.NewGuid(), null, "Calzado", null, isActive: true);
            _categoryRepository.GetByIdAsync(Arg.Any<ProductCategoryId>(), Arg.Any<IReadOnlyList<Guid>>(), Arg.Any<CancellationToken>()).Returns(category);
            _repository.ExistsByNameInCategoryAsync(Arg.Any<string>(), Arg.Any<ProductCategoryId>(), excludeId: Arg.Any<ProductSubCategoryId?>(), cancellationToken: Arg.Any<CancellationToken>())
                .Returns(false);

            var command = new CreateProductSubCategoryCommand(_categoryGuid, "Zapatillas", null);
            var result = await CreateHandler().Handle(command, CancellationToken.None);

            Assert.False(result.IsError);
            await _repository.Received(1).AddAsync(Arg.Any<ProductSubCategory>(), Arg.Any<CancellationToken>());
            await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }
    }
}
