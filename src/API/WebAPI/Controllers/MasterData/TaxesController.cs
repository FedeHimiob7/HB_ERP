using MasterData.Application.Taxes.Commands.CreateTax;
using MasterData.Application.Taxes.Commands.DeactivateTax;
using MasterData.Application.Taxes.Commands.RegisterTaxRate;
using MasterData.Application.Taxes.Commands.UpdateTaxDetails;
using MasterData.Application.Taxes.Queries.GetAll;
using MasterData.Application.Taxes.Queries.GetById;
using MasterData.Application.Taxes.Queries.GetEffectiveRate;
using MasterData.Application.Taxes.Queries.GetPaged;
using MasterData.Domain.SearchParametersModel;
using Microsoft.AspNetCore.Mvc;
using WebAPI.APIModels.MasterData.Tax;

namespace WebAPI.Controllers.MasterData
{
    [ApiController]
    [Route("api/[controller]")]
    public class TaxesController : ApiController
    {
        private readonly ISender _sender;
        public TaxesController(ISender sender) => _sender = sender;

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTaxRequest request)
        {
            var command = new CreateTaxCommand(request.Name, request.TaxType, request.Rate);
            var result = await _sender.Send(command);
            return result.Match(
                id => CreatedAtAction(nameof(GetById), new { id }, id),
                errors => Problem(errors));
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTaxRequest request)
        {
            var command = new UpdateTaxDetailsCommand(id, request.Name, request.TaxType);
            var result = await _sender.Send(command);
            return result.Match(
                taxResponse => Ok(taxResponse),
                errors => Problem(errors));
        }

        [HttpPost("{id:guid}/rates")]
        public async Task<IActionResult> RegisterRate(Guid id, [FromBody] RegisterTaxRateRequest request)
        {
            var command = new RegisterTaxRateCommand(id, request.Rate);
            var result = await _sender.Send(command);
            return result.Match(
                rateId => Ok(rateId),
                errors => Problem(errors));
        }

        [HttpGet("{id:guid}/effective")]
        public async Task<IActionResult> GetEffective(Guid id, [FromQuery] DateOnly date, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(new GetEffectiveTaxRateQuery(id, date), cancellationToken);
            return result.Match(rate => Ok(rate), errors => Problem(errors));
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Deactivate(Guid id)
        {
            var result = await _sender.Send(new DeactivateTaxCommand(id));
            return result.Match(_ => NoContent(), errors => Problem(errors));
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _sender.Send(new GetTaxByIdQuery(id));
            return result.Match(tax => Ok(tax), errors => Problem(errors));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var result = await _sender.Send(new GetAllTaxesQuery(), cancellationToken);
            return result.Match(taxes => Ok(taxes), errors => Problem(errors));
        }

        [HttpGet("paged")]
        public async Task<IActionResult> GetPaged([FromQuery] GetTaxesPagedRequest request, CancellationToken cancellationToken = default)
        {
            var filter = new TaxFilter(request.PageNumber, request.PageSize, request.SearchTerm, request.TaxType);
            var result = await _sender.Send(new GetTaxesPagedQuery(filter), cancellationToken);
            return result.Match(pagedResult => Ok(pagedResult), errors => Problem(errors));
        }
    }
}
