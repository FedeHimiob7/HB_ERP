using MasterData.Application.FiscalTerminals.Commands.UpdateFiscalTerminal;
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
    public sealed class UpdateFiscalTerminalCommandHandlerTests
    {
        private readonly IFiscalTerminalRepository _repository = Substitute.For<IFiscalTerminalRepository>();
        private readonly IMasterDataUnitOfWork _unitOfWork = Substitute.For<IMasterDataUnitOfWork>();
        private readonly Guid _terminalGuid = Guid.NewGuid();

        private UpdateFiscalTerminalCommandHandler CreateHandler() => new(_repository, _unitOfWork);

        [Fact]
        public async Task Handle_WhenFiscalTerminalDoesNotExist_ReturnsNotFound()
        {
            _repository.GetByIdAsync(Arg.Any<FiscalTerminalId>(), Arg.Any<CancellationToken>()).Returns((FiscalTerminal?)null);

            var command = new UpdateFiscalTerminalCommand(_terminalGuid, "Caja 1", EmissionMethod.Digital);
            var result = await CreateHandler().Handle(command, CancellationToken.None);

            Assert.True(result.IsError);
            Assert.Equal(FiscalTerminalErrors.NotFound.Code, result.FirstError.Code);
        }

        [Fact]
        public async Task Handle_WhenValid_UpdatesNameAndEmissionMethod()
        {
            // Empieza como MaquinaFiscal y el comando lo cambia a Digital — confirma que
            // UpdateDetails() realmente reasigna el medio de emisión, no solo el nombre.
            var fiscalTerminal = FiscalTerminal.CreateExisting(_terminalGuid, Guid.NewGuid(), "Caja Vieja", EmissionMethod.MaquinaFiscal, isActive: true);
            _repository.GetByIdAsync(Arg.Any<FiscalTerminalId>(), Arg.Any<CancellationToken>()).Returns(fiscalTerminal);

            var command = new UpdateFiscalTerminalCommand(_terminalGuid, "Caja Nueva", EmissionMethod.Digital);
            var result = await CreateHandler().Handle(command, CancellationToken.None);

            Assert.False(result.IsError);
            Assert.Equal("Caja Nueva", result.Value.Name);
            Assert.Equal(EmissionMethod.Digital, result.Value.EmissionMethod);
            Assert.Equal("Digital", result.Value.EmissionMethodName);

            await _repository.Received(1).UpdateAsync(fiscalTerminal, Arg.Any<CancellationToken>());
            await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }
    }
}
