using Identity.Application.SystemActions.Commands.Create;
using Identity.Application.SystemActions.Commands.DeleteSystemAction;
using Identity.Application.SystemActions.Commands.UpdateSystemAction;
using Identity.Application.SystemActions.Queries.GetAllSystemActions;
using Identity.Application.SystemActions.Queries.GetSystemActionById;
using Identity.Application.SystemActions.Queries.GetSystemActionsPaged;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPI.APIModels;
using WebAPI.APIModels.Identity.SystemActions;

namespace WebAPI.Controllers.Identity_Authenticate
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public sealed class ActionsController : ApiController
    {
        private readonly ISender _mediator;

        public ActionsController(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreateSystemActionRequest request,
            CancellationToken cancellationToken)
        {
            var command = new CreateSystemActionCommand(request.Name, request.Description);
            var result = await _mediator.Send(command, cancellationToken);

            return result.Match(
                id => Ok(new { Id = id }),
                errors => Problem(errors)
            );
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetAllSystemActionsQuery(), cancellationToken);

            return result.Match(
                actions => Ok(actions),
                errors => Problem(errors)
            );
        }

        [HttpGet("paged")]
        public async Task<IActionResult> GetPaged(
            [FromQuery] SearchParameters searchParameters,
            CancellationToken cancellationToken = default)
        {
            var query = new GetSystemActionsPagedQuery(
                searchParameters.PageNumber,
                searchParameters.PageSize);

            var result = await _mediator.Send(query, cancellationToken);

            return result.Match(
                paged => Ok(paged),
                errors => Problem(errors.ToList())
            );
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(
            [FromRoute] Guid id,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetSystemActionByIdQuery(id), cancellationToken);

            return result.Match(
                action => Ok(action),
                errors => Problem(errors)
            );
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(
            [FromRoute] Guid id,
            [FromBody] UpdateSystemActionRequest request,
            CancellationToken cancellationToken)
        {
            var command = new UpdateSystemActionCommand(id, request.Name, request.Description);
            var result = await _mediator.Send(command, cancellationToken);

            return result.Match(
                _ => NoContent(),
                errors => Problem(errors)
            );
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(
            [FromRoute] Guid id,
            CancellationToken cancellationToken)
        {
            var command = new DeleteSystemActionCommand(id);
            var result = await _mediator.Send(command, cancellationToken);

            return result.Match(
                _ => NoContent(),
                errors => Problem(errors)
            );
        }
    }
}
