using HB_ERP.SharedKernel.Domain.Primitives;
using MasterData.Application.Taxes.Queries.GetPaged;
using MasterData.Domain.Entities;
using MasterData.Domain.Enums;
using MasterData.Domain.Repositories;
using MasterData.Domain.SearchParametersModel;
using MasterData.Domain.VO;
using NSubstitute;
using Xunit;

namespace Application.Tests
{
    public sealed class GetTaxesPagedQueryHandlerTests
    {
        private readonly ITaxRepository _repository = Substitute.For<ITaxRepository>();
        private readonly IFiscalTaxRateRepository _fiscalTaxRateRepository = Substitute.For<IFiscalTaxRateRepository>();
        private readonly IFiscalClock _fiscalClock = Substitute.For<IFiscalClock>();

        private GetTaxesPagedQueryHandler CreateHandler() => new(_repository, _fiscalTaxRateRepository, _fiscalClock);

        [Fact]
        public async Task Handle_ReturnsPagedResultWithEffectiveRatesMapped()
        {
            var tax = Tax.Create("IGTF", TaxType.IGTF).Value;
            _repository.GetPagedAsync(Arg.Any<TaxFilter>(), Arg.Any<CancellationToken>())
                .Returns((new List<Tax> { tax }, 1));

            var rate = FiscalTaxRate.Create(tax.Id, 0.03m, DateTime.UtcNow).Value;
            _fiscalTaxRateRepository.GetEffectiveManyAsync(Arg.Any<IEnumerable<TaxId>>(), Arg.Any<DateOnly>(), Arg.Any<CancellationToken>())
                .Returns(new Dictionary<TaxId, FiscalTaxRate> { [tax.Id] = rate });

            var filter = new TaxFilter(1, 10, null, TaxType.IGTF);
            var result = await CreateHandler().Handle(new GetTaxesPagedQuery(filter), CancellationToken.None);

            Assert.False(result.IsError);
            Assert.Equal(1, result.Value.TotalCount);
            Assert.Equal(0.03m, result.Value.Items[0].Rate);
        }
    }
}
