using Identity.Application.Users.Commands.RegisterUser;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPI.DTOs.User.Request;

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
        [Route("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterUserRequest request)
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
    }
}
