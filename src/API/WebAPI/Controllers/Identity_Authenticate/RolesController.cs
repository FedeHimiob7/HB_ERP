using Identity.Application.Roles.Commands.AssignAction;
using Identity.Application.Roles.Commands.RegisterRole;
using Identity.Application.SystemActions.Commands.Create;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPI.APIModels.Identity.Authentication.SystemActions;
using WebAPI.APIModels.Identity.Role.Request;

namespace WebAPI.Controllers.Identity_Authenticate
{
    [Route("api/roles")]
    public sealed class RolesController : ApiController
    {
        private readonly ISender _mediator;
        public RolesController(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }


        [HttpPost]
        [Route("registerRole")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterRole(RegisterRoleRequest request)
        {
            var command = new RegisterRoleCommand(
                request.Name
            );

            var result = await _mediator.Send(command);

            return result.Match(
                id => Ok(id),
                errors => Problem(errors)
            );
        }

        [HttpPost]
        [Route("registerSystemAction")]
        public async Task<IActionResult> Create(
            [FromBody] CreateSystemActionRequest request,
            CancellationToken cancellationToken)
        {
            var command = new CreateSystemActionCommand(
                request.Name,
                request.Description);

            var result = await _mediator.Send(command, cancellationToken);

            return result.Match(
                systemActionId => Ok(new
                {
                    Id = systemActionId
                }),
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
