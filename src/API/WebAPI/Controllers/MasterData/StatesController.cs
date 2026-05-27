using MasterData.Application.States.Commands.CreateState;
using MasterData.Application.States.Commands.DeleteState;
using MasterData.Application.States.Commands.UpdateState;
using MasterData.Application.States.Queries.GetById;
using MasterData.Application.States.Queries.GetPaged;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPI.APIModels.MasterData.State;

namespace WebAPI.Controllers.MasterData
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class StatesController : ApiController
    {
        private readonly ISender _sender;

        public StatesController(ISender sender)
        {
            _sender = sender;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateStateRequest request, CancellationToken cancellationToken)
        {
            var command = new CreateStateCommand(request.CountryId, request.Code, request.Name);
            ErrorOr<Guid> result = await _sender.Send(command, cancellationToken);

            return result.Match(
                id => CreatedAtAction(nameof(GetById), new { id }, id),
                errors => Problem(errors)
            );
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateStateRequest request, CancellationToken cancellationToken)
        {
            var command = new UpdateStateCommand(id, request.CountryId, request.Code, request.Name);
            var result = await _sender.Send(command, cancellationToken);

            return result.Match(
                stateResponse => Ok(stateResponse),
                errors => Problem(errors)
            );
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Deactivate(Guid id, CancellationToken cancellationToken)
        {
            var command = new DeactivateStateCommand(id);
            ErrorOr<Success> result = await _sender.Send(command, cancellationToken);

            return result.Match(
                _ => NoContent(),
                errors => Problem(errors)
            );
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            var query = new GetStateByIdQuery(id);
            var result = await _sender.Send(query, cancellationToken);

            return result.Match(
                state => Ok(state),
                errors => Problem(errors)
            );
        }

        [HttpGet]
        public async Task<IActionResult> GetPaged(
                [FromQuery] GetStatesPagedRequest request,
                CancellationToken cancellationToken = default)
        {
            var query = new GetStatesPagedQuery(
                request.PageNumber,
                request.PageSize,
                request.CountryId,
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
