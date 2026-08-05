using ErrorOr;
using MasterData.Application.FiscalTerminals.Models;
using MasterData.Application.Interfaces;
using MasterData.Domain.DomainErrors;
using MasterData.Domain.Repositories;
using MasterData.Domain.VO;
using MediatR;

namespace MasterData.Application.FiscalTerminals.Commands.UpdateFiscalTerminal
{
    internal sealed class UpdateFiscalTerminalCommandHandler : IRequestHandler<UpdateFiscalTerminalCommand, ErrorOr<FiscalTerminalResponse>>
    {
        private readonly IFiscalTerminalRepository _repository;
        private readonly IMasterDataUnitOfWork _unitOfWork;

        public UpdateFiscalTerminalCommandHandler(IFiscalTerminalRepository repository, IMasterDataUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<FiscalTerminalResponse>> Handle(UpdateFiscalTerminalCommand request, CancellationToken cancellationToken)
        {
            var fiscalTerminal = await _repository.GetByIdAsync(FiscalTerminalId.Create(request.Id), cancellationToken);
            if (fiscalTerminal is null) return FiscalTerminalErrors.NotFound;

            var updateResult = fiscalTerminal.UpdateDetails(request.Name, request.EmissionMethod);
            if (updateResult.IsError) return updateResult.Errors;

            await _repository.UpdateAsync(fiscalTerminal, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new FiscalTerminalResponse(
                fiscalTerminal.Id.Value,
                fiscalTerminal.BranchId.Value,
                fiscalTerminal.Name,
                fiscalTerminal.EmissionMethod,
                fiscalTerminal.EmissionMethod.ToString());
        }
    }
}
