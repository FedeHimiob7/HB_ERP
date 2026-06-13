using Identity.Application.Roles.Commands.RegisterRole;
using Identity.Application.SystemActions.Commands.Create;
using Identity.Application.Users.Commands.DeleteUser;
using Identity.Application.Users.Commands.Login;
using Identity.Application.Users.Commands.RegisterUser;
using Identity.Application.Users.Commands.ResetUserPassword;
using Identity.Application.Users.Commands.UpdateUser;
using Identity.Application.Users.Queries.GetUserById;
using Identity.Application.Users.Queries.GetUsersPaged;
using Identity.Domain.VO;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using WebAPI.APIModels;
using WebAPI.APIModels.Identity.Authentication.User;

namespace WebAPI.Controllers.Identity_Authenticate
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticateController : ApiController
    {
        private readonly ISender _mediator;

        public AuthenticateController(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }


        [HttpPost]
        [Route("registerUser")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterUser(RegisterUserRequest request)
        {
            var command = new RegisterUserCommand(
                request.FirstName,
                request.LastName,
                request.Email,
                request.Password,
                request.RoleIds,
                request.PslIds
            );

            var result = await _mediator.Send(command);

            return result.Match(
                id => Ok(id),
                errors => Problem(errors)
            );
        }


        [HttpGet("users/{id:guid}")]
        public async Task<IActionResult> GetUserById(
            Guid id,
            CancellationToken cancellationToken)
        {
            var query = new GetUserByIdQuery(id);
            var result = await _mediator.Send(query, cancellationToken);

            return result.Match(
                user => Ok(user),
                errors => Problem(errors)
            );
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(
            [FromBody] LoginCommand request,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);

            return result.Match(
                token => Ok(new { Token = token }),
                errors => Problem(errors)
            );
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetUsersPaged(
            [FromQuery] SearchParameters searchParameters,
            CancellationToken cancellationToken = default)
        {
            var query = new GetUsersPagedQuery(
                searchParameters.PageNumber,
                searchParameters.PageSize);

            var result = await _mediator.Send(query, cancellationToken);

            return result.Match(
                pagedList => Ok(pagedList),
                errors => Problem(errors.ToList())
            );
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            var command = new DeleteUserCommand(id);
            var result = await _mediator.Send(command, cancellationToken);
            return result.Match(
                success => Ok(result),
                errors => Problem(errors)
            );
        }

        [HttpPatch("{id:guid}/password")]
        public async Task<IActionResult> ResetUserPassword(Guid id, [FromBody] ResetUserPasswordRequest request, CancellationToken cancellationToken)
        {
            var command = new ResetUserPasswordCommand(id, request.NewPassword);
            var result = await _mediator.Send(command, cancellationToken);

            return result.Match(
                _ => NoContent(),
                errors => Problem(errors)
            );
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserRequest request, CancellationToken cancellationToken)
        {
            var command = new UpdateUserCommand(
                id,
                request.FirstName,
                request.LastName,
                request.Email,
                request.RoleIds ?? new List<Guid>(),
                request.PslIds
            );

            var result = await _mediator.Send(command, cancellationToken);

            return result.Match(
                userId => Ok(userId),
                errors => Problem(errors)
            );
        }
    }
}
