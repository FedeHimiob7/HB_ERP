using HB_ERP.SharedKernel.Domain;
using HB_ERP.SharedKernel.Domain.Primitives;
using Inventory.Application.ProductCategories.Commands.CreateProductCategory;
using Inventory.Domain.DomainErrors;
using Inventory.Domain.Entities;
using Inventory.Domain.Repositories;
using MasterData.Domain.Entities;
using MasterData.Domain.Repositories;
using MasterData.Domain.VO;
using NSubstitute;
using Xunit;

namespace Application.Tests
{
    public sealed class CreateProductCategoryCommandHandlerTests
    {
        private readonly IProductCategoryRepository _repository = Substitute.For<IProductCategoryRepository>();
        private readonly IProductServiceLineRepository _pslRepository = Substitute.For<IProductServiceLineRepository>();
        private readonly IProductTypeRepository _productTypeRepository = Substitute.For<IProductTypeRepository>();
        private readonly Inventory.Application.Interfaces.IInventoryUnitOfWork _unitOfWork = Substitute.For<Inventory.Application.Interfaces.IInventoryUnitOfWork>();
        private readonly ICurrentUserProvider _currentUser = Substitute.For<ICurrentUserProvider>();
        private readonly Guid _pslGuid = Guid.NewGuid();

        private CreateProductCategoryCommandHandler CreateHandler()
            => new(_repository, _pslRepository, _productTypeRepository, _unitOfWork, _currentUser);

        [Fact]
        public async Task Handle_WhenPslNotInCurrentUserPsls_ReturnsPslAccessDenied()
        {
            _currentUser.PslIds.Returns(new List<Guid> { Guid.NewGuid() });

            var command = new CreateProductCategoryCommand(_pslGuid, null, "Calzado deportivo", null);
            var result = await CreateHandler().Handle(command, CancellationToken.None);

            Assert.True(result.IsError);
            Assert.Equal(CommonErrors.PslAccessDenied.Code, result.FirstError.Code);
        }

        [Fact]
        public async Task Handle_WhenNameAlreadyExistsInPsl_ReturnsDuplicateNameInPsl()
        {
            _currentUser.PslIds.Returns(new List<Guid> { _pslGuid });
            _pslRepository.GetByIdAsync(Arg.Any<ProductServiceLineId>(), Arg.Any<CancellationToken>())
                .Returns(ProductServiceLine.CreateExisting(_pslGuid, "desc", "Calzado", isActive: true));
            _repository.ExistsByNameInPslAsync("Calzado deportivo", Arg.Any<ProductServiceLineId>(), excludeId: Arg.Any<Inventory.Domain.VO.ProductCategoryId?>(), cancellationToken: Arg.Any<CancellationToken>())
                .Returns(true);

            var command = new CreateProductCategoryCommand(_pslGuid, null, "Calzado deportivo", null);
            var result = await CreateHandler().Handle(command, CancellationToken.None);

            Assert.True(result.IsError);
            Assert.Equal(ProductCategoryErrors.DuplicateNameInPsl.Code, result.FirstError.Code);
        }

        [Fact]
        public async Task Handle_WhenValid_PersistsAndReturnsId()
        {
            _currentUser.PslIds.Returns(new List<Guid> { _pslGuid });
            _pslRepository.GetByIdAsync(Arg.Any<ProductServiceLineId>(), Arg.Any<CancellationToken>())
                .Returns(ProductServiceLine.CreateExisting(_pslGuid, "desc", "Calzado", isActive: true));
            _repository.ExistsByNameInPslAsync(Arg.Any<string>(), Arg.Any<ProductServiceLineId>(), excludeId: Arg.Any<Inventory.Domain.VO.ProductCategoryId?>(), cancellationToken: Arg.Any<CancellationToken>())
                .Returns(false);

            var command = new CreateProductCategoryCommand(_pslGuid, null, "Calzado deportivo", null);
            var result = await CreateHandler().Handle(command, CancellationToken.None);

            Assert.False(result.IsError);
            await _repository.Received(1).AddAsync(Arg.Any<ProductCategory>(), Arg.Any<CancellationToken>());
            await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }
    }
}
