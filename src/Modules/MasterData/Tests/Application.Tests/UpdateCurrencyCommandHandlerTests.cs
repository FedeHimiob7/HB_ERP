using MasterData.Application.Currencies.Commands.CreateCurrencie;
using MasterData.Application.Currencies.Commands.UpdateCurrency;
using MasterData.Application.Interfaces;
using MasterData.Domain.Entities;
using MasterData.Domain.Repositories;
using MasterData.Domain.VO;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Xunit;

namespace Application.Tests
{
    public sealed class UpdateCurrencyCommandHandlerTests
    {
        private readonly ICurrencyRepository _repository = Substitute.For<ICurrencyRepository>();
        private readonly IMasterDataUnitOfWork _unitOfWork = Substitute.For<IMasterDataUnitOfWork>();
        // El logger real es ILogger<CreateCurrencyCommandHandler> por un detalle del código
        // (UpdateCurrencyCommandHandler reusa por error el tipo genérico de Create) — se mantiene
        // igual acá para que el constructor matchee. Se usa NullLogger en vez de un substitute
        // porque Castle no puede generar un proxy de ILogger<T> con un T `internal` (ver nota en
        // CreateCurrencyCommandHandlerTests).
        private readonly NullLogger<CreateCurrencyCommandHandler> _logger = NullLogger<CreateCurrencyCommandHandler>.Instance;
        private readonly Guid _currencyGuid = Guid.NewGuid();

        private UpdateCurrencyCommandHandler CreateHandler() => new(_repository, _unitOfWork, _logger);

        [Fact]
        public async Task Handle_WhenCurrencyDoesNotExist_ReturnsNotFound()
        {
            _repository.GetByIdAsync(Arg.Any<CurrencyId>(), Arg.Any<CancellationToken>()).Returns((Currency?)null);

            var command = new UpdateCurrencyCommand(_currencyGuid, "Dólar", "$");
            var result = await CreateHandler().Handle(command, CancellationToken.None);

            Assert.True(result.IsError);
            Assert.Equal("Currency.NotFound", result.FirstError.Code);
        }

        [Fact]
        public async Task Handle_WhenValid_UpdatesAndReturnsResponse()
        {
            var currency = Currency.CreateExisting(_currencyGuid, "USD", "Dolar Viejo", "US$", isActive: true);
            _repository.GetByIdAsync(Arg.Any<CurrencyId>(), Arg.Any<CancellationToken>()).Returns(currency);

            var command = new UpdateCurrencyCommand(_currencyGuid, "Dólar Estadounidense", "$");
            var result = await CreateHandler().Handle(command, CancellationToken.None);

            Assert.False(result.IsError);
            Assert.Equal("Dólar Estadounidense", result.Value.Name);
            Assert.Equal("$", result.Value.Symbol);
            // El Code NO se puede actualizar por este comando — sigue siendo el original.
            Assert.Equal("USD", result.Value.Code);

            await _repository.Received(1).UpdateAsync(currency, Arg.Any<CancellationToken>());
            await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }
    }
}
