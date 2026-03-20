using Identity.Application.Roles.Commands.RegisterRole;
using Identity.Application.Users.Commands.DeleteUser;
using Identity.Application.Users.Commands.RegisterUser;
using Identity.Application.Users.Queries.GetUserById;
using Identity.Application.Users.Queries.GetUsersPaged;
using Identity.Application.Users.Queries.Login;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPI.APIModels;
using WebAPI.APIModels.Autentication.User.Request;
using WebAPI.APIModels.Authentication.Role.Request;

namespace WebAPI.Controllers
{
    [Authorize]
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
                request.Password
            );

            var result = await _mediator.Send(command);

            return result.Match(
                id => Ok(id),
                errors => Problem(errors)
            );
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
            [FromBody] LoginQuery request,
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
                searchParameters.Page, 
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
    }
}
