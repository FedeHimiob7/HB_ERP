using ErrorOr;
using MasterData.Application.Interfaces;
using MasterData.Domain.DomainErrors;
using MasterData.Domain.Entities;
using MasterData.Domain.Repositories;
using MediatR;

namespace MasterData.Application.Companies.Commands.CreateCompany
{
    internal sealed class CreateCompanyCommandHandler : IRequestHandler<CreateCompanyCommand, ErrorOr<Guid>>
    {
        private readonly ICompanyRepository _repository;
        private readonly IMasterDataUnitOfWork _unitOfWork;

        public CreateCompanyCommandHandler(ICompanyRepository repository, IMasterDataUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Guid>> Handle(CreateCompanyCommand request, CancellationToken cancellationToken)
        {
            if (await _repository.ExistsAsync(cancellationToken))
                return CompanyErrors.AlreadyExists;

            var createResult = Company.Create(request.Rif, request.LegalName, request.RegisteredAddress, request.TaxpayerType);
            if (createResult.IsError) return createResult.Errors;

            var company = createResult.Value;

            await _repository.AddAsync(company, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return company.Id.Value;
        }
    }
}
