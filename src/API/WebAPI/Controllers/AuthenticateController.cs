using Identity.Application.Roles.Commands.RegisterRole;
using Identity.Application.Users.Commands.RegisterUser;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    }
}
