using MasterData.Application.Cities.Commands.CreateCity;
using MasterData.Application.Cities.Commands.DeactivateCity;
using MasterData.Application.Cities.Commands.UpdateCity;
using MasterData.Application.Cities.Queries.GetAll;
using MasterData.Application.Cities.Queries.GetById;
using MasterData.Application.Cities.Queries.GetPaged;
using MasterData.Domain.SearchParametersModel;
using MasterData.Domain.VO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPI.APIModels.MasterData.City;

namespace WebAPI.Controllers.MasterData
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class CitiesController : ApiController
    {
        private readonly ISender _sender;

        public CitiesController(ISender sender) => _sender = sender;

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCityRequest request, CancellationToken cancellationToken)
        {
            var command = new CreateCityCommand(request.StateId, request.Name);
            var result = await _sender.Send(command, cancellationToken);

            return result.Match(
                id => CreatedAtAction(nameof(GetById), new { id }, id),
                errors => Problem(errors));
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCityRequest request, CancellationToken cancellationToken)
        {
            var command = new UpdateCityCommand(id, request.StateId, request.Name);
            var result = await _sender.Send(command, cancellationToken);

            return result.Match(
                cityResponse => Ok(cityResponse),
                errors => Problem(errors));
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Deactivate(Guid id, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(new DeactivateCityCommand(id), cancellationToken);

            return result.Match(
                _ => NoContent(),
                errors => Problem(errors));
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(new GetCityByIdQuery(id), cancellationToken);

            return result.Match(
                city => Ok(city),
                errors => Problem(errors));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var result = await _sender.Send(new GetAllCitiesQuery(), cancellationToken);

            return result.Match(
                cities => Ok(cities),
                errors => Problem(errors));
        }

        [HttpGet("paged")]
        public async Task<IActionResult> GetPaged([FromQuery] GetCitiesPagedRequest request, CancellationToken cancellationToken)
        {
            StateId? stateId = request.StateId.HasValue
                ? StateId.Create(request.StateId.Value)
                : null;

            CountryId? countryId = request.CountryId.HasValue
                ? CountryId.Create(request.CountryId.Value)
                : null;

            var filter = new CityFilter(request.PageNumber, request.PageSize, request.SearchTerm, stateId, countryId);
            var result = await _sender.Send(new GetCitiesPagedQuery(filter), cancellationToken);

            return result.Match(
                pagedResult => Ok(pagedResult),
                errors => Problem(errors));
        }
    }
}
