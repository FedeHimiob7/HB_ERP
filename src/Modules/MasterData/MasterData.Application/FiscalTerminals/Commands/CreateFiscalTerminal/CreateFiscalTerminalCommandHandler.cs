using ErrorOr;
using MasterData.Application.Interfaces;
using MasterData.Domain.DomainErrors;
using MasterData.Domain.Entities;
using MasterData.Domain.Repositories;
using MasterData.Domain.VO;
using MediatR;

namespace MasterData.Application.FiscalTerminals.Commands.CreateFiscalTerminal
{
    internal sealed class CreateFiscalTerminalCommandHandler : IRequestHandler<CreateFiscalTerminalCommand, ErrorOr<Guid>>
    {
        private readonly IFiscalTerminalRepository _fiscalTerminalRepository;
        private readonly IBranchRepository _branchRepository;
        private readonly IMasterDataUnitOfWork _unitOfWork;

        public CreateFiscalTerminalCommandHandler(
            IFiscalTerminalRepository fiscalTerminalRepository,
            IBranchRepository branchRepository,
            IMasterDataUnitOfWork unitOfWork)
        {
            _fiscalTerminalRepository = fiscalTerminalRepository;
            _branchRepository = branchRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Guid>> Handle(CreateFiscalTerminalCommand request, CancellationToken cancellationToken)
        {
            var branchId = BranchId.Create(request.BranchId);

            var branch = await _branchRepository.GetByIdAsync(branchId, cancellationToken);
            if (branch is null)
                return FiscalTerminalErrors.InvalidBranch;

            var createResult = FiscalTerminal.Create(branchId, request.Name, request.EmissionMethod);
            if (createResult.IsError) return createResult.Errors;

            var fiscalTerminal = createResult.Value;

            await _fiscalTerminalRepository.AddAsync(fiscalTerminal, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return fiscalTerminal.Id.Value;
        }
    }
}
