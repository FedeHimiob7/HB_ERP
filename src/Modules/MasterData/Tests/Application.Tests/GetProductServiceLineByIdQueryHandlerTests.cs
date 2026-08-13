using HB_ERP.SharedKernel.Domain.Primitives;
using MasterData.Application.ProductServiceLines.Queries.GetById;
using MasterData.Domain.Entities;
using MasterData.Domain.Repositories;
using MasterData.Domain.VO;
using NSubstitute;
using Xunit;

namespace Application.Tests
{
    public sealed class GetProductServiceLineByIdQueryHandlerTests
    {
        private readonly IProductServiceLineRepository _repository = Substitute.For<IProductServiceLineRepository>();
        private readonly ICurrentUserProvider _currentUser = Substitute.For<ICurrentUserProvider>();
        private readonly Guid _pslGuid = Guid.NewGuid();

        private GetProductServiceLineByIdQueryHandler CreateHandler() => new(_repository, _currentUser);

        [Fact]
        public async Task Handle_WhenPslDoesNotExist_ReturnsNotFound()
        {
            _repository.GetByIdAsync(Arg.Any<ProductServiceLineId>(), Arg.Any<CancellationToken>()).Returns((ProductServiceLine?)null);

            var result = await CreateHandler().Handle(new GetProductServiceLineByIdQuery(_pslGuid), CancellationToken.None);

            Assert.True(result.IsError);
            Assert.Equal("ProductServiceLine.NotFound", result.FirstError.Code);
        }

        [Fact]
        public async Task Handle_WhenPslExistsButUserHasNoAccess_ReturnsNotFound()
        {
            // A diferencia de Inventory (CommonErrors.PslAccessDenied), acá el acceso denegado se
            // disfraza de NotFound — no revela que el PSL existe si el usuario no puede verlo.
            var psl = ProductServiceLine.CreateExisting(_pslGuid, "Desc", "Calzado", isActive: true);
            _repository.GetByIdAsync(Arg.Any<ProductServiceLineId>(), Arg.Any<CancellationToken>()).Returns(psl);
            _currentUser.PslIds.Returns(new List<Guid> { Guid.NewGuid() });

            var result = await CreateHandler().Handle(new GetProductServiceLineByIdQuery(_pslGuid), CancellationToken.None);

            Assert.True(result.IsError);
            Assert.Equal("ProductServiceLine.NotFound", result.FirstError.Code);
        }

        [Fact]
        public async Task Handle_WhenPslExistsAndUserHasAccess_ReturnsResponse()
        {
            var psl = ProductServiceLine.CreateExisting(_pslGuid, "Desc", "Calzado", isActive: true);
            _repository.GetByIdAsync(Arg.Any<ProductServiceLineId>(), Arg.Any<CancellationToken>()).Returns(psl);
            _currentUser.PslIds.Returns(new List<Guid> { _pslGuid });

            var result = await CreateHandler().Handle(new GetProductServiceLineByIdQuery(_pslGuid), CancellationToken.None);

            Assert.False(result.IsError);
            Assert.Equal(_pslGuid, result.Value.Id);
        }
    }
}
