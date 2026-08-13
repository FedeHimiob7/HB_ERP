using HB_ERP.SharedKernel.Domain.Primitives;
using Inventory.Application.ProductCategories.Commands.UpdateProductCategory;
using Inventory.Domain.DomainErrors;
using Inventory.Domain.Entities;
using Inventory.Domain.Repositories;
using Inventory.Domain.VO;
using MasterData.Domain.Entities;
using MasterData.Domain.Repositories;
using MasterData.Domain.VO;
using NSubstitute;
using Xunit;

namespace Application.Tests
{
    public sealed class UpdateProductCategoryCommandHandlerTests
    {
        private readonly IProductCategoryRepository _repository = Substitute.For<IProductCategoryRepository>();
        private readonly IProductServiceLineRepository _pslRepository = Substitute.For<IProductServiceLineRepository>();
        private readonly IProductTypeRepository _productTypeRepository = Substitute.For<IProductTypeRepository>();
        private readonly Inventory.Application.Interfaces.IInventoryUnitOfWork _unitOfWork = Substitute.For<Inventory.Application.Interfaces.IInventoryUnitOfWork>();
        private readonly ICurrentUserProvider _currentUser = Substitute.For<ICurrentUserProvider>();
        private readonly Guid _categoryGuid = Guid.NewGuid();
        private readonly Guid _pslGuid = Guid.NewGuid();

        private UpdateProductCategoryCommandHandler CreateHandler()
            => new(_repository, _pslRepository, _productTypeRepository, _unitOfWork, _currentUser);

        [Fact]
        public async Task Handle_WhenCategoryDoesNotExistOrNoPslAccess_ReturnsNotFound()
        {
            // GetByIdAsync ya filtra por allowedPslIds — si no hay acceso, el repo devuelve null
            // (mismo patrón que ProductServiceLine, no un error distinto de PslAccessDenied).
            _repository.GetByIdAsync(Arg.Any<ProductCategoryId>(), Arg.Any<IReadOnlyList<Guid>>(), Arg.Any<CancellationToken>())
                .Returns((ProductCategory?)null);

            var command = new UpdateProductCategoryCommand(_categoryGuid, _pslGuid, null, "Calzado deportivo", null);
            var result = await CreateHandler().Handle(command, CancellationToken.None);

            Assert.True(result.IsError);
            Assert.Equal(ProductCategoryErrors.NotFound.Code, result.FirstError.Code);
        }

        [Fact]
        public async Task Handle_WhenValid_UpdatesAndReturnsResponse()
        {
            var category = ProductCategory.CreateExisting(_categoryGuid, _pslGuid, null, "Calzado", null, isActive: true);
            _repository.GetByIdAsync(Arg.Any<ProductCategoryId>(), Arg.Any<IReadOnlyList<Guid>>(), Arg.Any<CancellationToken>()).Returns(category);
            _pslRepository.GetByIdAsync(Arg.Any<ProductServiceLineId>(), Arg.Any<CancellationToken>())
                .Returns(ProductServiceLine.CreateExisting(_pslGuid, "desc", "Calzado", isActive: true));
            _repository.ExistsByNameInPslAsync(Arg.Any<string>(), Arg.Any<ProductServiceLineId>(), excludeId: Arg.Any<ProductCategoryId?>(), cancellationToken: Arg.Any<CancellationToken>())
                .Returns(false);

            var command = new UpdateProductCategoryCommand(_categoryGuid, _pslGuid, null, "Calzado deportivo", null);
            var result = await CreateHandler().Handle(command, CancellationToken.None);

            Assert.False(result.IsError);
            Assert.Equal("Calzado deportivo", result.Value.Name);
            await _repository.Received(1).UpdateAsync(category, Arg.Any<CancellationToken>());
            await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }
    }
}
