using ErrorOr;
using MasterData.Application.FiscalTerminals.Models;
using MasterData.Domain.Repositories;
using MasterData.Domain.SearchParametersModel;
using MasterData.Domain.VO;
using MediatR;

namespace MasterData.Application.FiscalTerminals.Queries.GetPaged
{
    internal sealed class GetFiscalTerminalsPagedQueryHandler : IRequestHandler<GetFiscalTerminalsPagedQuery, ErrorOr<PagedFiscalTerminalsResult>>
    {
        private readonly IFiscalTerminalRepository _repository;

        public GetFiscalTerminalsPagedQueryHandler(IFiscalTerminalRepository repository) => _repository = repository;

        public async Task<ErrorOr<PagedFiscalTerminalsResult>> Handle(GetFiscalTerminalsPagedQuery request, CancellationToken cancellationToken)
        {
            BranchId? mappedBranchId = request.BranchId.HasValue
                ? BranchId.Create(request.BranchId.Value)
                : null;

            var filter = new FiscalTerminalFilter(
                request.PageNumber,
                request.PageSize,
                request.SearchTerm,
                mappedBranchId
            );

            var (fiscalTerminals, totalCount) = await _repository.GetPagedAsync(filter, cancellationToken);

            var mappedItems = fiscalTerminals.Select(f => new FiscalTerminalResponse(
                f.Id.Value,
                f.BranchId.Value,
                f.Name,
                f.EmissionMethod,
                f.EmissionMethod.ToString()
            )).ToList();

            return new PagedFiscalTerminalsResult(mappedItems, totalCount);
        }
    }
}
