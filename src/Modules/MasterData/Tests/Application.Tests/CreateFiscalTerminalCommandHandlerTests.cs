using MasterData.Application.FiscalTerminals.Commands.CreateFiscalTerminal;
using MasterData.Domain.DomainErrors;
using MasterData.Domain.Entities;
using MasterData.Domain.Enums;
using MasterData.Domain.Repositories;
using MasterData.Domain.VO;
using NSubstitute;
using Xunit;

namespace Application.Tests
{
    public sealed class CreateFiscalTerminalCommandHandlerTests
    {
        private readonly IFiscalTerminalRepository _fiscalTerminalRepository = Substitute.For<IFiscalTerminalRepository>();
        private readonly IBranchRepository _branchRepository = Substitute.For<IBranchRepository>();
        private readonly MasterData.Application.Interfaces.IMasterDataUnitOfWork _unitOfWork = Substitute.For<MasterData.Application.Interfaces.IMasterDataUnitOfWork>();
        private readonly Guid _branchGuid = Guid.NewGuid();

        private CreateFiscalTerminalCommandHandler CreateHandler() => new(_fiscalTerminalRepository, _branchRepository, _unitOfWork);

        [Fact]
        public async Task Handle_WhenBranchDoesNotExist_ReturnsInvalidBranch()
        {
            // D-1: FiscalTerminal siempre cuelga de una Branch existente — sin sucursal válida no
            // hay dónde configurar el punto de emisión.
            _branchRepository.GetByIdAsync(Arg.Any<BranchId>(), Arg.Any<CancellationToken>()).Returns((Branch?)null);

            var command = new CreateFiscalTerminalCommand(_branchGuid, "Caja 1", EmissionMethod.MaquinaFiscal);
            var result = await CreateHandler().Handle(command, CancellationToken.None);

            Assert.True(result.IsError);
            Assert.Equal(FiscalTerminalErrors.InvalidBranch.Code, result.FirstError.Code);
        }

        [Theory]
        [InlineData(EmissionMethod.MaquinaFiscal)]
        [InlineData(EmissionMethod.FormaLibre)]
        [InlineData(EmissionMethod.Digital)]
        public async Task Handle_WhenValid_CreatesFiscalTerminalWithGivenEmissionMethod(EmissionMethod emissionMethod)
        {
            // D-6: los 3 medios de emisión son de igual jerarquía — cada uno debe poder crearse
            // sin ningún tratamiento especial en el handler.
            var branch = Branch.CreateExisting(_branchGuid, Guid.NewGuid(), "Sucursal Test", "Direccion Test", 1, isActive: true);
            _branchRepository.GetByIdAsync(Arg.Any<BranchId>(), Arg.Any<CancellationToken>()).Returns(branch);

            var command = new CreateFiscalTerminalCommand(_branchGuid, "Caja 1", emissionMethod);
            var result = await CreateHandler().Handle(command, CancellationToken.None);

            Assert.False(result.IsError);
            Assert.NotEqual(Guid.Empty, result.Value);

            await _fiscalTerminalRepository.Received(1).AddAsync(
                Arg.Is<FiscalTerminal>(f => f.EmissionMethod == emissionMethod),
                Arg.Any<CancellationToken>());
            await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }
    }
}
