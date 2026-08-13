using MasterData.Application.Interfaces;
using MasterData.Application.ProductServiceLines.Commands.UpdatePSL;
using MasterData.Domain.Entities;
using MasterData.Domain.Repositories;
using MasterData.Domain.VO;
using NSubstitute;
using Xunit;

namespace Application.Tests
{
    public sealed class UpdateProductServiceLineCommandHandlerTests
    {
        private readonly IProductServiceLineRepository _repository = Substitute.For<IProductServiceLineRepository>();
        private readonly IMasterDataUnitOfWork _unitOfWork = Substitute.For<IMasterDataUnitOfWork>();
        private readonly Guid _pslGuid = Guid.NewGuid();

        private UpdateProductServiceLineCommandHandler CreateHandler() => new(_repository, _unitOfWork);

        [Fact]
        public async Task Handle_WhenPslDoesNotExist_ReturnsNotFound()
        {
            _repository.GetByIdAsync(Arg.Any<ProductServiceLineId>(), Arg.Any<CancellationToken>()).Returns((ProductServiceLine?)null);

            var command = new UpdateProductServiceLineCommand(_pslGuid, "Calzado", "Linea de calzado");
            var result = await CreateHandler().Handle(command, CancellationToken.None);

            Assert.True(result.IsError);
            Assert.Equal("ProductServiceLine.NotFound", result.FirstError.Code);
        }

        [Fact]
        public async Task Handle_WhenValid_UpdatesAndReturnsResponse()
        {
            var psl = ProductServiceLine.CreateExisting(_pslGuid, "Desc Vieja", "Nombre Viejo", isActive: true);
            _repository.GetByIdAsync(Arg.Any<ProductServiceLineId>(), Arg.Any<CancellationToken>()).Returns(psl);

            var command = new UpdateProductServiceLineCommand(_pslGuid, "Nombre Nuevo", "Desc Nueva");
            var result = await CreateHandler().Handle(command, CancellationToken.None);

            Assert.False(result.IsError);
            Assert.Equal("Nombre Nuevo", result.Value.Name);
            Assert.Equal("Desc Nueva", result.Value.Description);

            await _repository.Received(1).UpdateAsync(psl, Arg.Any<CancellationToken>());
            await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }
    }
}
