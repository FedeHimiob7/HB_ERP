using HB_ERP.SharedKernel.Domain.Primitives;
using MasterData.Application.Taxes.Queries.GetById;
using MasterData.Domain.DomainErrors;
using MasterData.Domain.Entities;
using MasterData.Domain.Enums;
using MasterData.Domain.Repositories;
using MasterData.Domain.VO;
using NSubstitute;
using Xunit;

namespace Application.Tests
{
    public sealed class GetTaxByIdQueryHandlerTests
    {
        private readonly ITaxRepository _repository = Substitute.For<ITaxRepository>();
        private readonly IFiscalTaxRateRepository _fiscalTaxRateRepository = Substitute.For<IFiscalTaxRateRepository>();
        private readonly IFiscalClock _fiscalClock = Substitute.For<IFiscalClock>();
        private readonly Guid _taxGuid = Guid.NewGuid();

        private GetTaxByIdQueryHandler CreateHandler() => new(_repository, _fiscalTaxRateRepository, _fiscalClock);

        [Fact]
        public async Task Handle_WhenTaxDoesNotExist_ReturnsNotFound()
        {
            _repository.GetByIdAsync(Arg.Any<TaxId>(), Arg.Any<CancellationToken>()).Returns((Tax?)null);

            var result = await CreateHandler().Handle(new GetTaxByIdQuery(_taxGuid), CancellationToken.None);

            Assert.True(result.IsError);
            Assert.Equal(TaxErrors.NotFound.Code, result.FirstError.Code);
        }

        [Fact]
        public async Task Handle_WhenTaxExists_ReturnsResponseWithEffectiveRate()
        {
            var tax = Tax.Create("IVA", TaxType.IVA).Value;
            _repository.GetByIdAsync(Arg.Any<TaxId>(), Arg.Any<CancellationToken>()).Returns(tax);

            var rate = FiscalTaxRate.Create(tax.Id, 0.16m, DateTime.UtcNow).Value;
            _fiscalTaxRateRepository.GetEffectiveAsync(tax.Id, Arg.Any<DateOnly>(), Arg.Any<CancellationToken>()).Returns(rate);

            var result = await CreateHandler().Handle(new GetTaxByIdQuery(_taxGuid), CancellationToken.None);

            Assert.False(result.IsError);
            Assert.Equal(0.16m, result.Value.Rate);
            Assert.Equal("IVA", result.Value.TaxTypeName);
        }
    }
}
