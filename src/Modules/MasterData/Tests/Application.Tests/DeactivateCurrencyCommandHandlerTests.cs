using MasterData.Application.Currencies.Commands.DeactivateCurrency;
using MasterData.Application.Interfaces;
using MasterData.Domain.Entities;
using MasterData.Domain.Repositories;
using MasterData.Domain.VO;
using NSubstitute;
using Xunit;

namespace Application.Tests
{
    public sealed class DeactivateCurrencyCommandHandlerTests
    {
        private readonly ICurrencyRepository _repository = Substitute.For<ICurrencyRepository>();
        private readonly IMasterDataUnitOfWork _unitOfWork = Substitute.For<IMasterDataUnitOfWork>();
        private readonly Guid _currencyGuid = Guid.NewGuid();

        private DeactivateCurrencyCommandHandler CreateHandler() => new(_repository, _unitOfWork);

        [Fact]
        public async Task Handle_WhenCurrencyDoesNotExist_ReturnsNotFound()
        {
            _repository.GetByIdAsync(Arg.Any<CurrencyId>(), Arg.Any<CancellationToken>()).Returns((Currency?)null);

            var result = await CreateHandler().Handle(new DeactivateCurrencyCommand(_currencyGuid), CancellationToken.None);

            Assert.True(result.IsError);
            Assert.Equal("Currency.NotFound", result.FirstError.Code);
        }

        [Fact]
        public async Task Handle_WhenValid_DeactivatesAndPersists()
        {
            var currency = Currency.CreateExisting(_currencyGuid, "USD", "Dólar", "$", isActive: true);
            _repository.GetByIdAsync(Arg.Any<CurrencyId>(), Arg.Any<CancellationToken>()).Returns(currency);

            var result = await CreateHandler().Handle(new DeactivateCurrencyCommand(_currencyGuid), CancellationToken.None);

            Assert.False(result.IsError);
            Assert.False(currency.IsActive);

            await _repository.Received(1).UpdateAsync(currency, Arg.Any<CancellationToken>());
            await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }
    }
}
