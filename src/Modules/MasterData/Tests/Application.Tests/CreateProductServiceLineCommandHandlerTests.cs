using MasterData.Application.Interfaces;
using MasterData.Application.ProductServiceLines.Commands.CreatePSL;
using MasterData.Domain.DomainErrors;
using MasterData.Domain.Entities;
using MasterData.Domain.Repositories;
using NSubstitute;
using Xunit;

namespace Application.Tests
{
    public sealed class CreateProductServiceLineCommandHandlerTests
    {
        private readonly IProductServiceLineRepository _repository = Substitute.For<IProductServiceLineRepository>();
        private readonly IMasterDataUnitOfWork _unitOfWork = Substitute.For<IMasterDataUnitOfWork>();

        private CreateProductServiceLineCommandHandler CreateHandler() => new(_repository, _unitOfWork);

        [Fact]
        public async Task Handle_WhenNameIsEmpty_ReturnsNameIsRequired()
        {
            var command = new CreateProductServiceLineCommand("", "Descripcion");
            var result = await CreateHandler().Handle(command, CancellationToken.None);

            Assert.True(result.IsError);
            Assert.Equal(ProductServiceLineErrors.NameIsRequired.Code, result.FirstError.Code);
        }

        [Fact]
        public async Task Handle_WhenValid_PersistsPslAndReturnsId()
        {
            var command = new CreateProductServiceLineCommand("Calzado", "Linea de calzado");
            var result = await CreateHandler().Handle(command, CancellationToken.None);

            Assert.False(result.IsError);
            Assert.NotEqual(Guid.Empty, result.Value);

            await _repository.Received(1).AddAsync(Arg.Any<ProductServiceLine>(), Arg.Any<CancellationToken>());
            await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }
    }
}
