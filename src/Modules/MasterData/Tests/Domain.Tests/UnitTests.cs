using Xunit;
using UnitEntity = MasterData.Domain.Entities.Unit;

namespace Domain.Tests
{
    // Se usa un alias porque "Unit" colisiona con MediatR.Unit y System.Reactive.Unit si alguna
    // vez se agrega ese using al archivo — mismo problema que ya resuelve un comentario en
    // CreateUnitCommandHandler ("le agregue el Domain.Entities por que hay un conflicto...").
    public sealed class UnitTests
    {
        [Fact]
        public void Create_WithValidData_Succeeds()
        {
            var result = UnitEntity.Create("Kilogramo", "Unidad de peso");

            Assert.False(result.IsError);
            Assert.Equal("Kilogramo", result.Value.Name);
            Assert.Equal("Unidad de peso", result.Value.Description);
            Assert.True(result.Value.IsActive);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Create_WithBlankName_Fails(string name)
        {
            var result = UnitEntity.Create(name, "Unidad de peso");

            Assert.True(result.IsError);
            Assert.Equal("Unit.NameIsRequired", result.FirstError.Code);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Create_WithBlankDescription_Fails(string description)
        {
            var result = UnitEntity.Create("Kilogramo", description);

            Assert.True(result.IsError);
            Assert.Equal("Unit.DescriptionIsRequired", result.FirstError.Code);
        }

        [Fact]
        public void UpdateDetails_WithValidData_UpdatesAllFields()
        {
            var unit = UnitEntity.Create("Kilogramo", "Unidad de peso").Value;

            var result = unit.UpdateDetails("Gramo", "Unidad de peso chica");

            Assert.False(result.IsError);
            Assert.Equal("Gramo", unit.Name);
            Assert.Equal("Unidad de peso chica", unit.Description);
        }

        [Fact]
        public void Deactivate_ThenActivate_TogglesIsActive()
        {
            var unit = UnitEntity.Create("Kilogramo", "Unidad de peso").Value;

            unit.Deactivate();
            Assert.False(unit.IsActive);

            unit.Activate();
            Assert.True(unit.IsActive);
        }
    }
}
