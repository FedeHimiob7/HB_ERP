using MasterData.Application.ExchangeRates.Commands.RegisterExchangeRate;
using MasterData.Application.ExchangeRates.Commands.SyncFromBCV;
using MasterData.Application.ExchangeRates.Queries.GetByDate;
using MasterData.Application.ExchangeRates.Queries.GetCurrent;
using MasterData.Application.ExchangeRates.Queries.GetPaged;
using Microsoft.AspNetCore.Mvc;
using WebAPI.APIModels.MasterData.ExchangeRate;

namespace WebAPI.Controllers.MasterData
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExchangeRatesController : ApiController
    {
        private readonly ISender _sender;
        public ExchangeRatesController(ISender sender) => _sender = sender;

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterExchangeRateRequest request)
        {
            var command = new RegisterExchangeRateCommand(request.Rate, request.Source);
            var result = await _sender.Send(command);
            return result.Match(
                id => CreatedAtAction(nameof(GetCurrent), id),
                errors => Problem(errors));
        }

        [HttpGet("current")]
        public async Task<IActionResult> GetCurrent(CancellationToken cancellationToken)
        {
            var result = await _sender.Send(new GetCurrentExchangeRateQuery(), cancellationToken);
            return result.Match(rate => Ok(rate), errors => Problem(errors));
        }

        [HttpGet("by-date")]
        public async Task<IActionResult> GetByDate([FromQuery] DateOnly date, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(new GetExchangeRateByDateQuery(date), cancellationToken);
            return result.Match(rate => Ok(rate), errors => Problem(errors));
        }

        [HttpGet("paged")]
        public async Task<IActionResult> GetPaged([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
        {
            var result = await _sender.Send(new GetExchangeRatesPagedQuery(pageNumber, pageSize), cancellationToken);
            return result.Match(pagedResult => Ok(pagedResult), errors => Problem(errors));
        }

        [HttpPost("sync-bcv")]
        public async Task<IActionResult> SyncFromBCV(CancellationToken cancellationToken)
        {
            var result = await _sender.Send(new SyncExchangeRateFromBCVCommand(), cancellationToken);
            return result.Match(syncResult => Ok(syncResult), errors => Problem(errors));
        }
    }
}
