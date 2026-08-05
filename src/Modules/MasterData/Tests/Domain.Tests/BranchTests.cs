using MasterData.Domain.Entities;
using MasterData.Domain.VO;
using Xunit;

namespace Domain.Tests
{
    public sealed class BranchTests
    {
        [Fact]
        public void Create_WithValidData_Succeeds()
        {
            var result = Branch.Create(CompanyId.Singleton, "Sucursal Centro", "Av. Principal, Caracas", 1);

            Assert.False(result.IsError);
            Assert.Equal("Sucursal Centro", result.Value.Name);
            Assert.Equal(1, result.Value.SequenceNumber);
            Assert.True(result.Value.IsActive);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Create_WithBlankName_Fails(string name)
        {
            var result = Branch.Create(CompanyId.Singleton, name, "Av. Principal, Caracas", 1);

            Assert.True(result.IsError);
            Assert.Equal("Branch.NameIsRequired", result.FirstError.Code);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Create_WithBlankAddress_Fails(string address)
        {
            var result = Branch.Create(CompanyId.Singleton, "Sucursal Centro", address, 1);

            Assert.True(result.IsError);
            Assert.Equal("Branch.AddressIsRequired", result.FirstError.Code);
        }

        [Fact]
        public void UpdateDetails_WithValidData_UpdatesAllFields()
        {
            var branch = Branch.Create(CompanyId.Singleton, "Sucursal Centro", "Av. Principal, Caracas", 1).Value;

            var result = branch.UpdateDetails("Sucursal Este", "Av. Este, Caracas");

            Assert.False(result.IsError);
            Assert.Equal("Sucursal Este", branch.Name);
            Assert.Equal("Av. Este, Caracas", branch.Address);
        }

        [Fact]
        public void UpdateDetails_WithBlankName_Fails()
        {
            var branch = Branch.Create(CompanyId.Singleton, "Sucursal Centro", "Av. Principal, Caracas", 1).Value;

            var result = branch.UpdateDetails("", "Av. Este, Caracas");

            Assert.True(result.IsError);
            Assert.Equal("Sucursal Centro", branch.Name);
        }

        [Fact]
        public void Deactivate_ThenActivate_TogglesIsActive()
        {
            var branch = Branch.Create(CompanyId.Singleton, "Sucursal Centro", "Av. Principal, Caracas", 1).Value;

            branch.Deactivate();
            Assert.False(branch.IsActive);

            branch.Activate();
            Assert.True(branch.IsActive);
        }
    }
}
