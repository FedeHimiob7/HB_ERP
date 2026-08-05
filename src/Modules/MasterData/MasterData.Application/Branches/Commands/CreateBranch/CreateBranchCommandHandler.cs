using ErrorOr;
using MasterData.Domain.DomainErrors;
using MasterData.Domain.Entities;
using MasterData.Domain.Repositories;
using MediatR;

namespace MasterData.Application.Branches.Commands.CreateBranch
{
    internal sealed class CreateBranchCommandHandler : IRequestHandler<CreateBranchCommand, ErrorOr<Guid>>
    {
        private readonly IBranchRepository _branchRepository;
        private readonly ICompanyRepository _companyRepository;

        public CreateBranchCommandHandler(IBranchRepository branchRepository, ICompanyRepository companyRepository)
        {
            _branchRepository = branchRepository;
            _companyRepository = companyRepository;
        }

        public async Task<ErrorOr<Guid>> Handle(CreateBranchCommand request, CancellationToken cancellationToken)
        {
            var company = await _companyRepository.GetSingletonAsync(cancellationToken);
            if (company is null) return CompanyErrors.NotConfigured;

            var result = await _branchRepository.ReserveNextSequenceNumberAndAddAsync(
                sequenceNumber => Branch.Create(company.Id, request.Name, request.Address, sequenceNumber),
                cancellationToken);

            if (result.IsError) return result.Errors;

            return result.Value.Id.Value;
        }
    }
}
