using Identity.Application.Roles.Commands.AssignAction;
using Identity.Application.Roles.Commands.DeleteRole;
using Identity.Application.Roles.Commands.RegisterRole;
using Identity.Application.Roles.Commands.UpdateRole;
using Identity.Application.Roles.Queries.GetAllRoles;
using Identity.Application.Roles.Queries.GetRolePagedQuery;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPI.APIModels;
using WebAPI.APIModels.Identity.Role;

namespace WebAPI.Controllers.Identity_Authenticate
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public sealed class RolesController : ApiController
    {
        private readonly ISender _mediator;
        public RolesController(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }


        [HttpPost]
        [Route("registerRole")]
        public async Task<IActionResult> RegisterRole(RegisterRoleRequest request)
        {
            var command = new RegisterRoleCommand(
                request.Name,
                request.ActionIds
            );

            var result = await _mediator.Send(command);

            return result.Match(
                id => Ok(id),
                errors => Problem(errors)
            );
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRoles(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetAllRolesQuery(), cancellationToken);

            return result.Match(
                roles => Ok(roles),
                errors => Problem(errors)
            );
        }

        [HttpGet("paged")]
        public async Task<IActionResult> GetRolesPaged(
            [FromQuery] SearchParameters searchParameters,
            CancellationToken cancellationToken = default)
        {
            var query = new GetRolesPagedQuery(
                searchParameters.PageNumber,
                searchParameters.PageSize);

            var result = await _mediator.Send(query, cancellationToken);

            return result.Match(
                pagedList => Ok(pagedList),
                errors => Problem(errors.ToList())
            );
        }


        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateRole(
            [FromRoute] Guid id,
            [FromBody] UpdateRoleRequest request,
            CancellationToken cancellationToken)
        {
            var command = new UpdateRoleCommand(id, request.Name, request.ActionIds);
            var result = await _mediator.Send(command, cancellationToken);

            return result.Match(
                _ => NoContent(),
                errors => Problem(errors)
            );
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteRole(
            [FromRoute] Guid id,
            CancellationToken cancellationToken)
        {
            var command = new DeleteRoleCommand(id);
            var result = await _mediator.Send(command, cancellationToken);

            return result.Match(
                _ => NoContent(),
                errors => Problem(errors)
            );
        }

        // Endpoint:  api/roles/{id}/actions
        [HttpPost("{id:guid}/actions")]
        public async Task<IActionResult> AssignAction(
            [FromRoute] Guid id, 
            [FromBody] AssignActionRequest request, 
            CancellationToken cancellationToken)
        {           
            var command = new AssignActionToRoleCommand(id, request.ActionId);
            var result = await _mediator.Send(command, cancellationToken);

            return result.Match(
                success => Ok(new { Message = "La acción fue asignada al rol correctamente." }),
                errors => Problem(errors)
            );
        }
    }
}
