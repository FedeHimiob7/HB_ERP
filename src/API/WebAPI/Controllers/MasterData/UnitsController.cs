using MasterData.Application.Units.Commands.CreateUnit;
using MasterData.Application.Units.Commands.DeleteUnit;
using MasterData.Application.Units.Commands.UpdateUnit;
using MasterData.Application.Units.Queries.GetAll;
using MasterData.Application.Units.Queries.GetById;
using MasterData.Application.Units.Queries.GetPaged;
using MasterData.Domain.SearchParametersModel;
using Microsoft.AspNetCore.Mvc;
using WebAPI.APIModels.MasterData.Unit;

namespace WebAPI.Controllers.MasterData
{
    [ApiController]
    [Route("api/[controller]")]
    public class UnitsController : ApiController
    {
        private readonly ISender _sender;

        public UnitsController(ISender sender) => _sender = sender;

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUnitRequest request)
        {
            var command = new CreateUnitCommand(request.Name, request.Description);
            var result = await _sender.Send(command);

            return result.Match(
                id => CreatedAtAction(nameof(GetById), new { id }, id),
                errors => Problem(errors));
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUnitRequest request)
        {
            var command = new UpdateUnitCommand(id, request.Name, request.Description);
            var result = await _sender.Send(command);

            return result.Match(
                unitResponse => Ok(unitResponse),
                errors => Problem(errors));
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Deactivate(Guid id)
        {
            var result = await _sender.Send(new DeactivateUnitCommand(id));
            return result.Match(_ => NoContent(), errors => Problem(errors));
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _sender.Send(new GetUnitByIdQuery(id));
            return result.Match(unit => Ok(unit), errors => Problem(errors));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var result = await _sender.Send(new GetAllUnitsQuery(), cancellationToken);
            return result.Match(units => Ok(units), errors => Problem(errors));
        }

        [HttpGet("paged")]
        public async Task<IActionResult> GetPaged([FromQuery] GetUnitsPagedRequest request, CancellationToken cancellationToken = default)
        {
            var filter = new UnitFilter(request.PageNumber, request.PageSize, request.SearchTerm);

            var result = await _sender.Send(new GetUnitsPagedQuery(filter), cancellationToken);
            return result.Match(pagedResult => Ok(pagedResult), errors => Problem(errors));
        }
    }
}
