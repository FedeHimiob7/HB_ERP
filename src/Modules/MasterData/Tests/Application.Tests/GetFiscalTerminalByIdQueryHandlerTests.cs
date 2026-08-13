using MasterData.Application.FiscalTerminals.Queries.GetById;
using MasterData.Domain.DomainErrors;
using MasterData.Domain.Entities;
using MasterData.Domain.Enums;
using MasterData.Domain.Repositories;
using MasterData.Domain.VO;
using NSubstitute;
using Xunit;

namespace Application.Tests
{
    public sealed class GetFiscalTerminalByIdQueryHandlerTests
    {
        private readonly IFiscalTerminalRepository _repository = Substitute.For<IFiscalTerminalRepository>();
        private readonly Guid _terminalGuid = Guid.NewGuid();

        private GetFiscalTerminalByIdQueryHandler CreateHandler() => new(_repository);

        [Fact]
        public async Task Handle_WhenNotFound_ReturnsNotFound()
        {
            _repository.GetByIdAsync(Arg.Any<FiscalTerminalId>(), Arg.Any<CancellationToken>()).Returns((FiscalTerminal?)null);

            var result = await CreateHandler().Handle(new GetFiscalTerminalByIdQuery(_terminalGuid), CancellationToken.None);

            Assert.True(result.IsError);
            Assert.Equal(FiscalTerminalErrors.NotFound.Code, result.FirstError.Code);
        }

        [Fact]
        public async Task Handle_WhenFound_ReturnsResponse()
        {
            var terminal = FiscalTerminal.CreateExisting(_terminalGuid, Guid.NewGuid(), "Caja 1", EmissionMethod.Digital, isActive: true);
            _repository.GetByIdAsync(Arg.Any<FiscalTerminalId>(), Arg.Any<CancellationToken>()).Returns(terminal);

            var result = await CreateHandler().Handle(new GetFiscalTerminalByIdQuery(_terminalGuid), CancellationToken.None);

            Assert.False(result.IsError);
            Assert.Equal(_terminalGuid, result.Value.Id);
            Assert.Equal(EmissionMethod.Digital, result.Value.EmissionMethod);
        }
    }
}
