using MasterData.Application.Companies.Queries.GetCurrent;
using MasterData.Domain.DomainErrors;
using MasterData.Domain.Entities;
using MasterData.Domain.Enums;
using MasterData.Domain.Repositories;
using NSubstitute;
using Xunit;

namespace Application.Tests
{
    public sealed class GetCurrentCompanyQueryHandlerTests
    {
        private readonly ICompanyRepository _repository = Substitute.For<ICompanyRepository>();

        private GetCurrentCompanyQueryHandler CreateHandler() => new(_repository);

        [Fact]
        public async Task Handle_WhenCompanyNotConfigured_ReturnsNotConfigured()
        {
            _repository.GetSingletonAsync(Arg.Any<CancellationToken>()).Returns((Company?)null);

            var result = await CreateHandler().Handle(new GetCurrentCompanyQuery(), CancellationToken.None);

            Assert.True(result.IsError);
            Assert.Equal(CompanyErrors.NotConfigured.Code, result.FirstError.Code);
        }

        [Fact]
        public async Task Handle_WhenConfigured_ReturnsCompanyResponse()
        {
            var company = Company.CreateExisting("J-401027631-4", "Empresa Test", "Direccion Test", TaxpayerType.Ordinario);
            _repository.GetSingletonAsync(Arg.Any<CancellationToken>()).Returns(company);

            var result = await CreateHandler().Handle(new GetCurrentCompanyQuery(), CancellationToken.None);

            Assert.False(result.IsError);
            Assert.Equal(company.Id.Value, result.Value.Id);
            Assert.Equal("J-401027631-4", result.Value.Rif);
            Assert.Equal(TaxpayerType.Ordinario, result.Value.TaxpayerType);
            // TaxpayerTypeName es el string legible del enum, no solo el número.
            Assert.Equal("Ordinario", result.Value.TaxpayerTypeName);
        }
    }
}
