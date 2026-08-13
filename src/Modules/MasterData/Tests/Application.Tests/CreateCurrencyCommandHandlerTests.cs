using MasterData.Application.Currencies.Commands.CreateCurrencie;
using MasterData.Application.Interfaces;
using MasterData.Domain.Entities;
using MasterData.Domain.Repositories;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Xunit;

namespace Application.Tests
{
    public sealed class CreateCurrencyCommandHandlerTests
    {
        private readonly ICurrencyRepository _repository = Substitute.For<ICurrencyRepository>();
        private readonly IMasterDataUnitOfWork _unitOfWork = Substitute.For<IMasterDataUnitOfWork>();
        // NSubstitute no puede generar un proxy de ILogger<T> cuando T es un tipo `internal`
        // (Castle DynamicProxy choca con el ensamblado fuertemente firmado de Logging.Abstractions).
        // Como acá no verificamos nada del logger, usamos el logger real "no-op" en vez de mockearlo.
        private readonly NullLogger<CreateCurrencyCommandHandler> _logger = NullLogger<CreateCurrencyCommandHandler>.Instance;

        private CreateCurrencyCommandHandler CreateHandler() => new(_repository, _unitOfWork, _logger);

        [Fact]
        public async Task Handle_WhenCodeAlreadyExists_ReturnsConflict()
        {
            _repository.ExistsByCodeAsync("USD", Arg.Any<CancellationToken>()).Returns(true);

            var command = new CreateCurrencyCommand("USD", "Dólar", "$");
            var result = await CreateHandler().Handle(command, CancellationToken.None);

            Assert.True(result.IsError);
            Assert.Equal("Currency.Duplicate", result.FirstError.Code);
            await _repository.DidNotReceive().AddAsync(Arg.Any<Currency>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_WhenCodeIsNotThreeCharacters_ReturnsInvalidCode()
        {
            // El guard de "no duplicado" pasa (repo dice que no existe), pero el dominio rechaza
            // el código por longitud — confirma que ambas validaciones son independientes.
            _repository.ExistsByCodeAsync("US", Arg.Any<CancellationToken>()).Returns(false);

            var command = new CreateCurrencyCommand("US", "Dólar", "$");
            var result = await CreateHandler().Handle(command, CancellationToken.None);

            Assert.True(result.IsError);
            Assert.Equal("Currency.InvalidCode", result.FirstError.Code);
        }

        [Fact]
        public async Task Handle_WhenValid_PersistsCurrencyAndReturnsId()
        {
            _repository.ExistsByCodeAsync("USD", Arg.Any<CancellationToken>()).Returns(false);

            var command = new CreateCurrencyCommand("USD", "Dólar", "$");
            var result = await CreateHandler().Handle(command, CancellationToken.None);

            Assert.False(result.IsError);
            Assert.NotEqual(Guid.Empty, result.Value);

            await _repository.Received(1).AddAsync(Arg.Any<Currency>(), Arg.Any<CancellationToken>());
            await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }
    }
}
