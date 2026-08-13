using MasterData.Application.FiscalTerminals.Commands.DeleteFiscalTerminal;
using MasterData.Application.Interfaces;
using MasterData.Domain.DomainErrors;
using MasterData.Domain.Entities;
using MasterData.Domain.Enums;
using MasterData.Domain.Repositories;
using MasterData.Domain.VO;
using NSubstitute;
using Xunit;

namespace Application.Tests
{
    public sealed class DeactivateFiscalTerminalCommandHandlerTests
    {
        private readonly IFiscalTerminalRepository _repository = Substitute.For<IFiscalTerminalRepository>();
        private readonly IMasterDataUnitOfWork _unitOfWork = Substitute.For<IMasterDataUnitOfWork>();
        private readonly Guid _terminalGuid = Guid.NewGuid();

        private DeactivateFiscalTerminalCommandHandler CreateHandler() => new(_repository, _unitOfWork);

        [Fact]
        public async Task Handle_WhenFiscalTerminalDoesNotExist_ReturnsNotFound()
        {
            _repository.GetByIdAsync(Arg.Any<FiscalTerminalId>(), Arg.Any<CancellationToken>()).Returns((FiscalTerminal?)null);

            var result = await CreateHandler().Handle(new DeactivateFiscalTerminalCommand(_terminalGuid), CancellationToken.None);

            Assert.True(result.IsError);
            Assert.Equal(FiscalTerminalErrors.NotFound.Code, result.FirstError.Code);
        }

        [Fact]
        public async Task Handle_WhenValid_DeactivatesAndPersists()
        {
            var fiscalTerminal = FiscalTerminal.CreateExisting(_terminalGuid, Guid.NewGuid(), "Caja 1", EmissionMethod.FormaLibre, isActive: true);
            _repository.GetByIdAsync(Arg.Any<FiscalTerminalId>(), Arg.Any<CancellationToken>()).Returns(fiscalTerminal);

            var result = await CreateHandler().Handle(new DeactivateFiscalTerminalCommand(_terminalGuid), CancellationToken.None);

            Assert.False(result.IsError);
            Assert.False(fiscalTerminal.IsActive);

            await _repository.Received(1).UpdateAsync(fiscalTerminal, Arg.Any<CancellationToken>());
            await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }
    }
}
