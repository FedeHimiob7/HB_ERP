using ErrorOr;
using MasterData.Application.Companies.Models;
using MasterData.Application.Interfaces;
using MasterData.Domain.DomainErrors;
using MasterData.Domain.Repositories;
using MediatR;

namespace MasterData.Application.Companies.Commands.UpdateCompany
{
    internal sealed class UpdateCompanyCommandHandler : IRequestHandler<UpdateCompanyCommand, ErrorOr<CompanyResponse>>
    {
        private readonly ICompanyRepository _repository;
        private readonly IMasterDataUnitOfWork _unitOfWork;

        public UpdateCompanyCommandHandler(ICompanyRepository repository, IMasterDataUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<CompanyResponse>> Handle(UpdateCompanyCommand request, CancellationToken cancellationToken)
        {
            var company = await _repository.GetSingletonAsync(cancellationToken);
            if (company is null) return CompanyErrors.NotConfigured;

            var updateResult = company.UpdateDetails(request.Rif, request.LegalName, request.RegisteredAddress, request.TaxpayerType);
            if (updateResult.IsError) return updateResult.Errors;

            await _repository.UpdateAsync(company, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

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
