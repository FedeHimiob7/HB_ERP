using ErrorOr;
using MasterData.Application.FiscalTerminals.Models;
using MediatR;

namespace MasterData.Application.FiscalTerminals.Queries.GetAll
{
    public record GetAllFiscalTerminalsQuery(Guid? BranchId = null) : IRequest<ErrorOr<IReadOnlyList<FiscalTerminalResponse>>>;
}
