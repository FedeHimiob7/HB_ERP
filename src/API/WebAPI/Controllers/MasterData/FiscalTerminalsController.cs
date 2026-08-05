using MasterData.Application.FiscalTerminals.Commands.CreateFiscalTerminal;
using MasterData.Application.FiscalTerminals.Commands.DeleteFiscalTerminal;
using MasterData.Application.FiscalTerminals.Commands.UpdateFiscalTerminal;
using MasterData.Application.FiscalTerminals.Queries.GetAll;
using MasterData.Application.FiscalTerminals.Queries.GetById;
using MasterData.Application.FiscalTerminals.Queries.GetPaged;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPI.APIModels.MasterData.FiscalTerminal;

namespace WebAPI.Controllers.MasterData
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class FiscalTerminalsController : ApiController
    {
        private readonly ISender _sender;

        public FiscalTerminalsController(ISender sender)
        {
            _sender = sender;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateFiscalTerminalRequest request, CancellationToken cancellationToken)
        {
            var command = new CreateFiscalTerminalCommand(request.BranchId, request.Name, request.EmissionMethod);
            ErrorOr<Guid> result = await _sender.Send(command, cancellationToken);

            return result.Match(
                id => CreatedAtAction(nameof(GetById), new { id }, id),
                errors => Problem(errors)
            );
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateFiscalTerminalRequest request, CancellationToken cancellationToken)
        {
            var command = new UpdateFiscalTerminalCommand(id, request.Name, request.EmissionMethod);
            var result = await _sender.Send(command, cancellationToken);

            return result.Match(
                fiscalTerminalResponse => Ok(fiscalTerminalResponse),
                errors => Problem(errors)
            );
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Deactivate(Guid id, CancellationToken cancellationToken)
        {
            var command = new DeactivateFiscalTerminalCommand(id);
            ErrorOr<Success> result = await _sender.Send(command, cancellationToken);

            return result.Match(
                _ => NoContent(),
                errors => Problem(errors)
            );
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            var query = new GetFiscalTerminalByIdQuery(id);
            var result = await _sender.Send(query, cancellationToken);

            return result.Match(
                fiscalTerminal => Ok(fiscalTerminal),
                errors => Problem(errors)
            );
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] Guid? branchId = null,
            CancellationToken cancellationToken = default)
        {
            var result = await _sender.Send(new GetAllFiscalTerminalsQuery(branchId), cancellationToken);

            return result.Match(
                fiscalTerminals => Ok(fiscalTerminals),
                errors => Problem(errors));
        }

        [HttpGet("paged")]
        public async Task<IActionResult> GetPaged(
                [FromQuery] GetFiscalTerminalsPagedRequest request,
                CancellationToken cancellationToken = default)
        {
            var query = new GetFiscalTerminalsPagedQuery(
                request.PageNumber,
                request.PageSize,
                request.BranchId,
                request.SearchTerm
            );

            var result = await _sender.Send(query, cancellationToken);

            return result.Match(
                pagedResult => Ok(pagedResult),
                errors => Problem(errors)
            );
        }
    }
}
