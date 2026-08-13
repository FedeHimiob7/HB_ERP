using MasterData.Application.Branches.Queries.GetPaged;
using MasterData.Domain.Entities;
using MasterData.Domain.Repositories;
using MasterData.Domain.SearchParametersModel;
using NSubstitute;
using Xunit;

namespace Application.Tests
{
    public sealed class GetBranchesPagedQueryHandlerTests
    {
        private readonly IBranchRepository _repository = Substitute.For<IBranchRepository>();

        private GetBranchesPagedQueryHandler CreateHandler() => new(_repository);

        [Fact]
        public async Task Handle_ReturnsPagedResultWithTotalCount()
        {
            var companyGuid = Guid.NewGuid();
            var branches = new List<Branch>
            {
                Branch.CreateExisting(Guid.NewGuid(), companyGuid, "Sucursal 1", "Direccion 1", 1, isActive: true),
            };
            // El repo es quien pagina de verdad (SQL); acá solo verificamos que el handler arma el
            // filtro correcto y mapea lo que el repo le devuelve, incluyendo el TotalCount real
            // (que puede ser mayor a la cantidad de items de esta página).
            _repository.GetPagedAsync(Arg.Any<BranchFilter>(), Arg.Any<CancellationToken>())
                .Returns((branches, 25));

            var result = await CreateHandler().Handle(new GetBranchesPagedQuery(1, 10, "Sucursal"), CancellationToken.None);

            Assert.False(result.IsError);
            Assert.Single(result.Value.Items);
            Assert.Equal(25, result.Value.TotalCount);

            // Confirmamos que el SearchTerm del comando efectivamente llegó al filtro pasado al repo.
            await _repository.Received(1).GetPagedAsync(
                Arg.Is<BranchFilter>(f => f.SearchTerm == "Sucursal" && f.PageNumber == 1 && f.PageSize == 10),
                Arg.Any<CancellationToken>());
        }
    }
}
