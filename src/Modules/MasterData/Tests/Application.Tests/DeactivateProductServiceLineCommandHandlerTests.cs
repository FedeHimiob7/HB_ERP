using MasterData.Application.Interfaces;
using MasterData.Application.ProductServiceLines.Commands.DesactivatePSL;
using MasterData.Domain.Entities;
using MasterData.Domain.Repositories;
using MasterData.Domain.VO;
using NSubstitute;
using Xunit;

namespace Application.Tests
{
    public sealed class DeactivateProductServiceLineCommandHandlerTests
    {
        private readonly IProductServiceLineRepository _repository = Substitute.For<IProductServiceLineRepository>();
        private readonly IMasterDataUnitOfWork _unitOfWork = Substitute.For<IMasterDataUnitOfWork>();
        private readonly Guid _pslGuid = Guid.NewGuid();

        private DeactivateProductServiceLineCommandHandler CreateHandler() => new(_repository, _unitOfWork);

        [Fact]
        public async Task Handle_WhenPslDoesNotExist_ReturnsNotFound()
        {
            _repository.GetByIdAsync(Arg.Any<ProductServiceLineId>(), Arg.Any<CancellationToken>()).Returns((ProductServiceLine?)null);

            var result = await CreateHandler().Handle(new DeactivateProductServiceLineCommand(_pslGuid), CancellationToken.None);

            Assert.True(result.IsError);
            Assert.Equal("ProductServiceLine.NotFound", result.FirstError.Code);
        }

        [Fact]
        public async Task Handle_WhenValid_DeactivatesAndPersists()
        {
            var psl = ProductServiceLine.CreateExisting(_pslGuid, "Desc", "Calzado", isActive: true);
            _repository.GetByIdAsync(Arg.Any<ProductServiceLineId>(), Arg.Any<CancellationToken>()).Returns(psl);

            var result = await CreateHandler().Handle(new DeactivateProductServiceLineCommand(_pslGuid), CancellationToken.None);

            Assert.False(result.IsError);
            Assert.False(psl.IsActive);

            await _repository.Received(1).UpdateAsync(psl, Arg.Any<CancellationToken>());
            await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }
    }
}
