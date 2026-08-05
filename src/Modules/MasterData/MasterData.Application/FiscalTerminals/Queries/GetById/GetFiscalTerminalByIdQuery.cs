using ErrorOr;
using MasterData.Application.FiscalTerminals.Models;
using MediatR;

namespace MasterData.Application.FiscalTerminals.Queries.GetById
{
    public record GetFiscalTerminalByIdQuery(Guid Id) : IRequest<ErrorOr<FiscalTerminalResponse>>;
}
