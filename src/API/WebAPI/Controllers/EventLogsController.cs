using Identity.Application.EventLogs.Queries.GetEventLogPaged;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPI.APIModels.Identity.EventLog;

namespace WebAPI.Controllers
{
    // Fachada HTTP compartida para los EventLog de todos los módulos que tengan uno.
    // Cada módulo mantiene su propia tabla/query — este controller solo unifica el
    // namespace de rutas (api/EventLogs/{modulo}/paged) para que el UI tenga un único
    // lugar donde buscar auditoría, sin que eso implique compartir DbContext entre módulos.
    [Authorize]
    [ApiController]
    [Route("api/EventLogs")]
    public class EventLogsController : ApiController
    {
        private readonly ISender _mediator;

        public EventLogsController(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        // Identity / Security-IAM: login, altas/bajas de usuarios, cambios de roles y permisos.
        [HttpGet("security/paged")]
        public async Task<IActionResult> GetSecurityPaged(
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

        // Próxima acción a agregar acá cuando exista (F3): GetFiscalPaged → api/EventLogs/fiscal/paged,
        // delegando a la query de EventLog del módulo Billing/FiscalDocument.
    }
}
