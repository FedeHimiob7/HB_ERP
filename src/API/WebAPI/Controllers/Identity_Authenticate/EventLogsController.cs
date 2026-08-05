using Identity.Application.EventLogs.Queries.GetEventLogPaged;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPI.APIModels.Identity.EventLog;

namespace WebAPI.Controllers.Identity_Authenticate
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class EventLogsController : ApiController
    {
        private readonly ISender _mediator;

        public EventLogsController(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpGet("paged")]
        public async Task<IActionResult> GetPaged(
            [FromQuery] GetEventLogPagedRequest request,
            CancellationToken cancellationToken = default)
        {
            var query = new GetEventLogPagedQuery(
                request.PageNumber,
                request.PageSize,
                request.Type,
                request.UserId,
                request.From,
                request.To
            );

            var result = await _mediator.Send(query, cancellationToken);

            return result.Match(
                pagedList => Ok(pagedList),
                errors => Problem(errors.ToList())
            );
        }
    }
}
