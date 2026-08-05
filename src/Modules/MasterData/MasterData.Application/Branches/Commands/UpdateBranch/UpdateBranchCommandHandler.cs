using ErrorOr;
using MasterData.Application.Branches.Models;
using MasterData.Application.Interfaces;
using MasterData.Domain.DomainErrors;
using MasterData.Domain.Repositories;
using MasterData.Domain.VO;
using MediatR;

namespace MasterData.Application.Branches.Commands.UpdateBranch
{
    internal sealed class UpdateBranchCommandHandler : IRequestHandler<UpdateBranchCommand, ErrorOr<BranchResponse>>
    {
        private readonly IBranchRepository _repository;
        private readonly IMasterDataUnitOfWork _unitOfWork;

        public UpdateBranchCommandHandler(IBranchRepository repository, IMasterDataUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<BranchResponse>> Handle(UpdateBranchCommand request, CancellationToken cancellationToken)
        {
            var branch = await _repository.GetByIdAsync(BranchId.Create(request.Id), cancellationToken);
            if (branch is null) return BranchErrors.NotFound;

            var updateResult = branch.UpdateDetails(request.Name, request.Address);
            if (updateResult.IsError) return updateResult.Errors;

            await _repository.UpdateAsync(branch, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new BranchResponse(branch.Id.Value, branch.CompanyId.Value, branch.Name, branch.Address, branch.SequenceNumber);
        }
    }
}
