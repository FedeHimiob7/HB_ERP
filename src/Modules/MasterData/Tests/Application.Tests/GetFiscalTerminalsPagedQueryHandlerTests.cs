using MasterData.Application.FiscalTerminals.Queries.GetPaged;
using MasterData.Domain.Entities;
using MasterData.Domain.Enums;
using MasterData.Domain.Repositories;
using MasterData.Domain.SearchParametersModel;
using NSubstitute;
using Xunit;

namespace Application.Tests
{
    public sealed class GetFiscalTerminalsPagedQueryHandlerTests
    {
        private readonly IFiscalTerminalRepository _repository = Substitute.For<IFiscalTerminalRepository>();

        private GetFiscalTerminalsPagedQueryHandler CreateHandler() => new(_repository);

        [Fact]
        public async Task Handle_BuildsFilterWithBranchIdAndReturnsPagedResult()
        {
            var branchGuid = Guid.NewGuid();
            var terminals = new List<FiscalTerminal>
            {
                FiscalTerminal.CreateExisting(Guid.NewGuid(), branchGuid, "Caja 1", EmissionMethod.MaquinaFiscal, isActive: true),
            };
            _repository.GetPagedAsync(Arg.Any<FiscalTerminalFilter>(), Arg.Any<CancellationToken>())
                .Returns((terminals, 4));

            var query = new GetFiscalTerminalsPagedQuery(1, 10, branchGuid, "Caja");
            var result = await CreateHandler().Handle(query, CancellationToken.None);

            Assert.False(result.IsError);
            Assert.Single(result.Value.Items);
            Assert.Equal(4, result.Value.TotalCount);

            // El filtro extra (BranchId) es la diferencia de FiscalTerminal contra Branch —
            // confirmamos que efectivamente se propaga al filtro pasado al repo.
            await _repository.Received(1).GetPagedAsync(
                Arg.Is<FiscalTerminalFilter>(f => f.BranchId != null && f.BranchId.Value.Value == branchGuid && f.SearchTerm == "Caja"),
                Arg.Any<CancellationToken>());
        }
    }
}
