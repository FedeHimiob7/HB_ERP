using HB_ERP.SharedKernel.Domain.Primitives;
using Inventory.Application.ProductSubCategories.Queries.GetAll;
using Inventory.Domain.Entities;
using Inventory.Domain.Repositories;
using Inventory.Domain.VO;
using NSubstitute;
using Xunit;

namespace Application.Tests
{
    public sealed class GetAllProductSubCategoriesQueryHandlerTests
    {
        private readonly IProductSubCategoryRepository _repository = Substitute.For<IProductSubCategoryRepository>();
        private readonly ICurrentUserProvider _currentUser = Substitute.For<ICurrentUserProvider>();

        private GetAllProductSubCategoriesQueryHandler CreateHandler() => new(_repository, _currentUser);

        [Fact]
        public async Task Handle_PassesCurrentUserAllowedPslIdsToRepository()
        {
            var allowedIds = new List<Guid> { Guid.NewGuid() };
            _currentUser.PslIds.Returns(allowedIds);
            _repository.GetAllAsync(Arg.Any<IReadOnlyList<Guid>>(), Arg.Any<ProductCategoryId?>(), Arg.Any<CancellationToken>())
                .Returns(new List<ProductSubCategory>());

            await CreateHandler().Handle(new GetAllProductSubCategoriesQuery(), CancellationToken.None);

            await _repository.Received(1).GetAllAsync(allowedIds, Arg.Any<ProductCategoryId?>(), Arg.Any<CancellationToken>());
        }
    }
}
