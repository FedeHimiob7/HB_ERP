using ErrorOr;
using MasterData.Application.FiscalTerminals.Models;
using MasterData.Domain.DomainErrors;
using MasterData.Domain.Repositories;
using MasterData.Domain.VO;
using MediatR;

namespace MasterData.Application.FiscalTerminals.Queries.GetById
{
    internal sealed class GetFiscalTerminalByIdQueryHandler : IRequestHandler<GetFiscalTerminalByIdQuery, ErrorOr<FiscalTerminalResponse>>
    {
        private readonly IFiscalTerminalRepository _repository;

        public GetFiscalTerminalByIdQueryHandler(IFiscalTerminalRepository repository) => _repository = repository;

        public async Task<ErrorOr<FiscalTerminalResponse>> Handle(GetFiscalTerminalByIdQuery request, CancellationToken cancellationToken)
        {
            var fiscalTerminal = await _repository.GetByIdAsync(FiscalTerminalId.Create(request.Id), cancellationToken);

            if (fiscalTerminal is null) return FiscalTerminalErrors.NotFound;

            return new FiscalTerminalResponse(
                fiscalTerminal.Id.Value,
                fiscalTerminal.BranchId.Value,
                fiscalTerminal.Name,
                fiscalTerminal.EmissionMethod,
                fiscalTerminal.EmissionMethod.ToString());
        }
    }
}
