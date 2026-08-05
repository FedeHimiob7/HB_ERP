using MasterData.Domain.Entities;
using MasterData.Domain.Enums;
using MasterData.Domain.VO;
using Xunit;

namespace Domain.Tests
{
    public sealed class FiscalTerminalTests
    {
        [Fact]
        public void Create_WithValidData_Succeeds()
        {
            var result = FiscalTerminal.Create(BranchId.New(), "Caja 1", EmissionMethod.MaquinaFiscal);

            Assert.False(result.IsError);
            Assert.Equal("Caja 1", result.Value.Name);
            Assert.Equal(EmissionMethod.MaquinaFiscal, result.Value.EmissionMethod);
            Assert.True(result.Value.IsActive);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Create_WithBlankName_Fails(string name)
        {
            var result = FiscalTerminal.Create(BranchId.New(), name, EmissionMethod.MaquinaFiscal);

            Assert.True(result.IsError);
            Assert.Equal("FiscalTerminal.NameIsRequired", result.FirstError.Code);
        }

        [Fact]
        public void Create_WithEmptyBranchId_Fails()
        {
            var result = FiscalTerminal.Create(BranchId.Create(Guid.Empty), "Caja 1", EmissionMethod.MaquinaFiscal);

            Assert.True(result.IsError);
            Assert.Equal("FiscalTerminal.InvalidBranch", result.FirstError.Code);
        }

        [Fact]
        public void UpdateDetails_WithValidData_UpdatesAllFields()
        {
            var fiscalTerminal = FiscalTerminal.Create(BranchId.New(), "Caja 1", EmissionMethod.MaquinaFiscal).Value;

            var result = fiscalTerminal.UpdateDetails("POS Web", EmissionMethod.Digital);

            Assert.False(result.IsError);
            Assert.Equal("POS Web", fiscalTerminal.Name);
            Assert.Equal(EmissionMethod.Digital, fiscalTerminal.EmissionMethod);
        }

        [Fact]
        public void UpdateDetails_WithBlankName_Fails()
        {
            var fiscalTerminal = FiscalTerminal.Create(BranchId.New(), "Caja 1", EmissionMethod.MaquinaFiscal).Value;

            var result = fiscalTerminal.UpdateDetails("", EmissionMethod.Digital);

            Assert.True(result.IsError);
            Assert.Equal("Caja 1", fiscalTerminal.Name);
        }

        [Fact]
        public void Deactivate_ThenActivate_TogglesIsActive()
        {
            var fiscalTerminal = FiscalTerminal.Create(BranchId.New(), "Caja 1", EmissionMethod.MaquinaFiscal).Value;

            fiscalTerminal.Deactivate();
            Assert.False(fiscalTerminal.IsActive);

            fiscalTerminal.Activate();
            Assert.True(fiscalTerminal.IsActive);
        }
    }
}
