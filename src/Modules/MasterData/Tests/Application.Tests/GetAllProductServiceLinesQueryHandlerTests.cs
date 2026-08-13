using HB_ERP.SharedKernel.Domain.Primitives;
using MasterData.Application.ProductServiceLines.Queries.GetAll;
using MasterData.Domain.Entities;
using MasterData.Domain.Repositories;
using NSubstitute;
using Xunit;

namespace Application.Tests
{
    public sealed class GetAllProductServiceLinesQueryHandlerTests
    {
        // Convención de filtro PSL (ver feedback_psl_filter_standard): el repositorio recibe
        // allowedPslIds del usuario actual — nunca devuelve PSLs a los que el usuario no tiene acceso.
        private readonly IProductServiceLineRepository _repository = Substitute.For<IProductServiceLineRepository>();
        private readonly ICurrentUserProvider _currentUser = Substitute.For<ICurrentUserProvider>();

        private GetAllProductServiceLinesQueryHandler CreateHandler() => new(_repository, _currentUser);

        [Fact]
        public async Task Handle_PassesCurrentUserAllowedPslIdsToRepository()
        {
            var allowedIds = new List<Guid> { Guid.NewGuid() };
            _currentUser.PslIds.Returns(allowedIds);
            _repository.GetAllAsync(Arg.Any<IReadOnlyList<Guid>>(), Arg.Any<CancellationToken>()).Returns(new List<ProductServiceLine>());

            await CreateHandler().Handle(new GetAllProductServiceLinesQuery(), CancellationToken.None);

            await _repository.Received(1).GetAllAsync(allowedIds, Arg.Any<CancellationToken>());
        }
    }
}
