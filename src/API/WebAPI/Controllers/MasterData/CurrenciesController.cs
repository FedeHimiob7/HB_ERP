using MasterData.Application.Currencies.Commands;
using MasterData.Application.Currencies.Commands.CreateCurrencie;
using MasterData.Application.Currencies.Commands.DeactivateCurrency;
using MasterData.Application.Currencies.Commands.UpdateCurrency;
using MasterData.Application.Currencies.Queries.GetCurrencies;
using MasterData.Application.Currencies.Queries.GetCurrencyById;
using MasterData.Application.Currencies.Queries.GetPaged;
using MasterData.Domain.SearchParametersModel;
using Microsoft.AspNetCore.Authorization;
using WebAPI.APIModels.MasterData.Currencies;

namespace WebAPI.Controllers.MasterData
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class CurrenciesController : ApiController
    {
        private readonly ISender _sender;

        public CurrenciesController(ISender sender)
        {
            _sender = sender;
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreateCurrencyRequest request,
            CancellationToken cancellationToken)
        {
            var command = new CreateCurrencyCommand(request.Code, request.Name, request.Symbol);

            var result = await _sender.Send(command, cancellationToken);

            return result.Match(
                currencyId => Ok(new
                {
                    Id = currencyId,
                }),
                errors => Problem(errors)
            );
        }

        [HttpGet("paged")]
        public async Task<IActionResult> GetPaged(
            [FromQuery] GetCurrenciesPagedRequest request,
            CancellationToken cancellationToken = default)
        {
            var filter = new CurrencyFilter(request.PageNumber, request.PageSize, request.SearchTerm);
            var query = new GetCurrenciesPagedQuery(filter);
            var result = await _sender.Send(query, cancellationToken);

            return result.Match(
                pagedResult => Ok(pagedResult),
                errors => Problem(errors)
            );
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var query = new GetAllCurrenciesQuery();

            var result = await _sender.Send(query, cancellationToken);

            return result.Match(
                currencies => Ok(currencies),
                errors => Problem(errors)
            );
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            var query = new GetCurrencyByIdQuery(id);

            var result = await _sender.Send(query, cancellationToken);

            return result.Match(
                currency => Ok(currency),
                errors => Problem(errors)
            );
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(
            Guid id,
            [FromBody] UpdateCurrencyRequest request,
            CancellationToken cancellationToken)
        {
            var command = new UpdateCurrencyCommand(id, request.Name, request.Symbol);

            var result = await _sender.Send(command, cancellationToken);

            return result.Match(
                updatedCurrency => Ok(updatedCurrency),
                errors => Problem(errors)
            );
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            var command = new DeactivateCurrencyCommand(id);

            var result = await _sender.Send(command, cancellationToken);

            return result.Match(
                success => NoContent(),
                errors => Problem(errors)
            );
        }

    }
}
