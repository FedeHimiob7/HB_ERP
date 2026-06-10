using MasterData.Application.Countries.Commands.CreateCountry;
using MasterData.Application.Countries.Commands.DeleteCountry;
using MasterData.Application.Countries.Commands.UpdateCountry;
using MasterData.Application.Countries.Models;
using MasterData.Application.Countries.Queries.GetAllCountries;
using MasterData.Application.Countries.Queries.GetCountryById;
using MasterData.Application.Countries.Queries.GetPagedCountry;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPI.APIModels.MasterData.Country;

namespace WebAPI.Controllers.MasterData
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class CountriesController : ApiController
    {
        private readonly ISender _sender;

        public CountriesController(ISender sender)
        {
            _sender = sender;
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreateCountryRequest request,
            CancellationToken cancellationToken)
        {
            var command = new CreateCountryCommand(request.Name);
            ErrorOr<Guid> result = await _sender.Send(command, cancellationToken);

            return result.Match(
                id => CreatedAtAction(nameof(GetById), new { id }, id),
                errors => Problem(errors)
            );
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(
            Guid id,
            [FromBody] UpdateCountryRequest request,
            CancellationToken cancellationToken)
        {
            var command = new UpdateCountryCommand(id, request.Name);
            ErrorOr<CountryResponse> result = await _sender.Send(command, cancellationToken);

            return result.Match(
                result => Ok(result),
                errors => Problem(errors)
            );
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Deactivate(Guid id, CancellationToken cancellationToken)
        {
            var command = new DeactivateCountryCommand(id);
            ErrorOr<Success> result = await _sender.Send(command, cancellationToken);

            return result.Match(
                _ => NoContent(),
                errors => Problem(errors)
            );
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            var query = new GetCountryByIdQuery(id);
            var result = await _sender.Send(query, cancellationToken);

            return result.Match(
                country => Ok(country),
                errors => Problem(errors)
            );
        }

        [HttpGet("paged")]
        public async Task<IActionResult> GetPaged(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? searchTerm = null,
            CancellationToken cancellationToken = default)
        {
            var query = new GetCountriesPagedQuery(pageNumber, pageSize, searchTerm);
            var result = await _sender.Send(query, cancellationToken);

            return result.Match(
                pagedResult => Ok(pagedResult),
                errors => Problem(errors)
            );
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            // Se instancia la query sin parámetros
            var query = new GetAllCountriesQuery();

            var result = await _sender.Send(query, cancellationToken);

            return result.Match(
                countries => Ok(countries),
                errors => Problem(errors)
            );
        }
    }
}
