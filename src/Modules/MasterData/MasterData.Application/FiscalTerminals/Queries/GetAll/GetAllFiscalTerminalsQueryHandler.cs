using ErrorOr;
using MasterData.Application.FiscalTerminals.Models;
using MasterData.Domain.Repositories;
using MasterData.Domain.VO;
using MediatR;

namespace MasterData.Application.FiscalTerminals.Queries.GetAll
{
    internal sealed class GetAllFiscalTerminalsQueryHandler
        : IRequestHandler<GetAllFiscalTerminalsQuery, ErrorOr<IReadOnlyList<FiscalTerminalResponse>>>
    {
        private readonly IFiscalTerminalRepository _repository;

        public GetAllFiscalTerminalsQueryHandler(IFiscalTerminalRepository repository) => _repository = repository;

        public async Task<ErrorOr<IReadOnlyList<FiscalTerminalResponse>>> Handle(
            GetAllFiscalTerminalsQuery request,
            CancellationToken cancellationToken)
        {
            BranchId? branchId = request.BranchId.HasValue
                ? BranchId.Create(request.BranchId.Value)
                : null;

            var fiscalTerminals = await _repository.GetAllAsync(branchId, cancellationToken);

            var response = fiscalTerminals
                .Select(f => new FiscalTerminalResponse(f.Id.Value, f.BranchId.Value, f.Name, f.EmissionMethod, f.EmissionMethod.ToString()))
                .ToList();

            return response;
        }
    }
}
