using MasterData.Application.FiscalTerminals.Queries.GetAll;
using MasterData.Domain.Entities;
using MasterData.Domain.Enums;
using MasterData.Domain.Repositories;
using MasterData.Domain.VO;
using NSubstitute;
using Xunit;

namespace Application.Tests
{
    public sealed class GetAllFiscalTerminalsQueryHandlerTests
    {
        private readonly IFiscalTerminalRepository _repository = Substitute.For<IFiscalTerminalRepository>();

        private GetAllFiscalTerminalsQueryHandler CreateHandler() => new(_repository);

        [Fact]
        public async Task Handle_WhenBranchIdProvided_PassesItToRepository()
        {
            // El filtro por sucursal es opcional (Guid? BranchId) — cuando viene, el handler debe
            // convertirlo a BranchId y pasarlo tal cual al repo (no ignorarlo).
            var branchGuid = Guid.NewGuid();
            _repository.GetAllAsync(Arg.Any<BranchId?>(), Arg.Any<CancellationToken>())
                .Returns(new List<FiscalTerminal>());

            await CreateHandler().Handle(new GetAllFiscalTerminalsQuery(branchGuid), CancellationToken.None);

            await _repository.Received(1).GetAllAsync(
                Arg.Is<BranchId?>(id => id == BranchId.Create(branchGuid)),
                Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_MapsFiscalTerminalsToResponseWithEmissionMethodName()
        {
            var branchGuid = Guid.NewGuid();
            var terminals = new List<FiscalTerminal>
            {
                FiscalTerminal.CreateExisting(Guid.NewGuid(), branchGuid, "Caja 1", EmissionMethod.MaquinaFiscal, isActive: true),
            };
            _repository.GetAllAsync(Arg.Any<BranchId?>(), Arg.Any<CancellationToken>()).Returns(terminals);

            var result = await CreateHandler().Handle(new GetAllFiscalTerminalsQuery(null), CancellationToken.None);

            Assert.False(result.IsError);
            Assert.Single(result.Value);
            Assert.Equal("MaquinaFiscal", result.Value[0].EmissionMethodName);
        }
    }
}
