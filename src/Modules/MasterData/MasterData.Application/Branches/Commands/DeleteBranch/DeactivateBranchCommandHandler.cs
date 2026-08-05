using ErrorOr;
using MasterData.Application.Interfaces;
using MasterData.Domain.DomainErrors;
using MasterData.Domain.Repositories;
using MasterData.Domain.VO;
using MediatR;

namespace MasterData.Application.Branches.Commands.DeleteBranch
{
    internal sealed class DeactivateBranchCommandHandler : IRequestHandler<DeactivateBranchCommand, ErrorOr<Success>>
    {
        private readonly IBranchRepository _repository;
        private readonly IMasterDataUnitOfWork _unitOfWork;

        public DeactivateBranchCommandHandler(IBranchRepository repository, IMasterDataUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Success>> Handle(DeactivateBranchCommand request, CancellationToken cancellationToken)
        {
            var branch = await _repository.GetByIdAsync(BranchId.Create(request.Id), cancellationToken);

            if (branch is null) return BranchErrors.NotFound;

            branch.Deactivate();

            await _repository.UpdateAsync(branch, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success;
        }
    }
}
