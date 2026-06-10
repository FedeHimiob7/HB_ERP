using MasterData.Application.ProductServiceLines.Commands.CreatePSL;
using MasterData.Application.ProductServiceLines.Commands.DesactivatePSL;
using MasterData.Application.ProductServiceLines.Commands.UpdatePSL;
using MasterData.Application.ProductServiceLines.Models;
using MasterData.Application.ProductServiceLines.Queries.GetAll;
using MasterData.Application.ProductServiceLines.Queries.GetById;
using MasterData.Application.ProductServiceLines.Queries.GetPaged;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPI.APIModels.MasterData.ProductServiceLine;

namespace WebAPI.Controllers.MasterData
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class ProductServiceLinesController : ApiController
    {
        private readonly ISender _sender;

        public ProductServiceLinesController(ISender sender)
        {
            _sender = sender;
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreateProductServiceLineRequest request,
            CancellationToken cancellationToken)
        {
            var command = new CreateProductServiceLineCommand(request.Name, request.Description);

            ErrorOr<Guid> result = await _sender.Send(command, cancellationToken);

            return result.Match(
                id => CreatedAtAction(nameof(GetById), new { id }, id),
                errors => Problem(errors)
            );
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(
            Guid id,
            [FromBody] UpdateProductServiceLineRequest request,
            CancellationToken cancellationToken)
        {
            var command = new UpdateProductServiceLineCommand(id, request.Name, request.Description);

            ErrorOr<ProductServiceLineResponse> result = await _sender.Send(command, cancellationToken);

            return result.Match(
                updatePSL => Ok(result.Value),
                errors => Problem(errors)
            );
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Deactivate(Guid id, CancellationToken cancellationToken)
        {
            var command = new DeactivateProductServiceLineCommand(id);

            ErrorOr<Success> result = await _sender.Send(command, cancellationToken);

            return result.Match(
                _ => NoContent(),
                errors => Problem(errors)
            );
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            var query = new GetProductServiceLineByIdQuery(id);

            var result = await _sender.Send(query, cancellationToken);

            return result.Match(
                pslResponse => Ok(pslResponse),
                errors => Problem(errors)
            );
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var query = new GetAllProductServiceLinesQuery();
            var result = await _sender.Send(query, cancellationToken);

            return result.Match(
                lines => Ok(lines),
                errors => Problem(errors)
            );
        }

        [HttpGet("paged")]
        public async Task<IActionResult> GetPaged(
            [FromQuery] GetProductServiceLinesPagedRequest request,
            CancellationToken cancellationToken = default)
        {
            var query = new GetProductServiceLinesPagedQuery(request.PageNumber, request.PageSize, request.SearchTerm);

            var result = await _sender.Send(query, cancellationToken);

            return result.Match(
                pagedResult => Ok(pagedResult),
                errors => Problem(errors)
            );
        }
    }
}