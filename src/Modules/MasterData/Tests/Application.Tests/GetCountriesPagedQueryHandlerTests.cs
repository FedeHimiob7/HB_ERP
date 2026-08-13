using MasterData.Application.Countries.Queries.GetPagedCountry;
using MasterData.Domain.Entities;
using MasterData.Domain.Repositories;
using NSubstitute;
using Xunit;

namespace Application.Tests
{
    public sealed class GetCountriesPagedQueryHandlerTests
    {
        private readonly ICountryRepository _repository = Substitute.For<ICountryRepository>();

        private GetCountriesPagedQueryHandler CreateHandler() => new(_repository);

        [Fact]
        public async Task Handle_PassesPagingArgumentsAndReturnsMappedResult()
        {
            // Este repo (a diferencia de Branch/FiscalTerminal) no usa un objeto Filter, recibe los
            // parámetros sueltos — confirmamos que el handler los reenvía tal cual, sin transformarlos.
            var countries = new List<Country> { Country.CreateExisting(Guid.NewGuid(), "Venezuela", isActive: true) };
            _repository.GetPagedAsync(2, 5, "Vene", Arg.Any<CancellationToken>()).Returns((countries, 11));

            var result = await CreateHandler().Handle(new GetCountriesPagedQuery(2, 5, "Vene"), CancellationToken.None);

            Assert.False(result.IsError);
            Assert.Single(result.Value.Items);
            Assert.Equal(11, result.Value.TotalCount);
        }
    }
}
