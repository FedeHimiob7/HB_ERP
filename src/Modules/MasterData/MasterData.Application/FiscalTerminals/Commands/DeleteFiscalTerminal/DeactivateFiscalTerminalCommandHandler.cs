using ErrorOr;
using MasterData.Application.Interfaces;
using MasterData.Domain.DomainErrors;
using MasterData.Domain.Repositories;
using MasterData.Domain.VO;
using MediatR;

namespace MasterData.Application.FiscalTerminals.Commands.DeleteFiscalTerminal
{
    internal sealed class DeactivateFiscalTerminalCommandHandler : IRequestHandler<DeactivateFiscalTerminalCommand, ErrorOr<Success>>
    {
        private readonly IFiscalTerminalRepository _repository;
        private readonly IMasterDataUnitOfWork _unitOfWork;

        public DeactivateFiscalTerminalCommandHandler(IFiscalTerminalRepository repository, IMasterDataUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Success>> Handle(DeactivateFiscalTerminalCommand request, CancellationToken cancellationToken)
        {
            var fiscalTerminal = await _repository.GetByIdAsync(FiscalTerminalId.Create(request.Id), cancellationToken);

            if (fiscalTerminal is null) return FiscalTerminalErrors.NotFound;

            fiscalTerminal.Deactivate();

            await _repository.UpdateAsync(fiscalTerminal, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success;
        }
    }
}
