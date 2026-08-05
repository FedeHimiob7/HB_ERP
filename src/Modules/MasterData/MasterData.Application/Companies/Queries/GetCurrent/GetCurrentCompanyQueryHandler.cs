using ErrorOr;
using MasterData.Application.Companies.Models;
using MasterData.Domain.DomainErrors;
using MasterData.Domain.Repositories;
using MediatR;

namespace MasterData.Application.Companies.Queries.GetCurrent
{
    internal sealed class GetCurrentCompanyQueryHandler : IRequestHandler<GetCurrentCompanyQuery, ErrorOr<CompanyResponse>>
    {
        private readonly ICompanyRepository _repository;

        public GetCurrentCompanyQueryHandler(ICompanyRepository repository) => _repository = repository;

        public async Task<ErrorOr<CompanyResponse>> Handle(GetCurrentCompanyQuery request, CancellationToken cancellationToken)
        {
            var company = await _repository.GetSingletonAsync(cancellationToken);
            if (company is null) return CompanyErrors.NotConfigured;

            return new CompanyResponse(
                company.Id.Value,
                company.Rif,
                company.LegalName,
                company.RegisteredAddress,
                company.TaxpayerType,
                company.TaxpayerType.ToString());
        }
    }
}
