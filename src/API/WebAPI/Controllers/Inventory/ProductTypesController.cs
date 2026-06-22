using Inventory.Application.ProductTypes.Commands.CreateProductType;
using Inventory.Application.ProductTypes.Commands.DeactivateProductType;
using Inventory.Application.ProductTypes.Commands.UpdateProductType;
using Inventory.Application.ProductTypes.Queries.GetAll;
using Inventory.Application.ProductTypes.Queries.GetById;
using Inventory.Application.ProductTypes.Queries.GetPaged;
using Inventory.Domain.SearchParametersModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPI.APIModels.Inventory.ProductType;

namespace WebAPI.Controllers.Inventory
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class ProductTypesController : ApiController
    {
        private readonly ISender _sender;

        public ProductTypesController(ISender sender)
        {
            _sender = sender;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProductTypeRequest request, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(new CreateProductTypeCommand(request.Name, request.Description), cancellationToken);
            return result.Match(
                id => CreatedAtAction(nameof(GetById), new { id }, new { Id = id }),
                errors => Problem(errors));
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProductTypeRequest request, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(new UpdateProductTypeCommand(id, request.Name, request.Description), cancellationToken);
            return result.Match(
                updated => Ok(updated),
                errors => Problem(errors));
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Deactivate(Guid id, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(new DeactivateProductTypeCommand(id), cancellationToken);
            return result.Match(
                _ => NoContent(),
                errors => Problem(errors));
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(new GetProductTypeByIdQuery(id), cancellationToken);
            return result.Match(
                item => Ok(item),
                errors => Problem(errors));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var result = await _sender.Send(new GetAllProductTypesQuery(), cancellationToken);
            return result.Match(
                items => Ok(items),
                errors => Problem(errors));
        }

        [HttpGet("paged")]
        public async Task<IActionResult> GetPaged([FromQuery] GetProductTypesPagedRequest request, CancellationToken cancellationToken)
        {
            var filter = new ProductTypeFilter(request.PageNumber, request.PageSize, request.SearchTerm);
            var result = await _sender.Send(new GetProductTypesPagedQuery(filter), cancellationToken);
            return result.Match(
                paged => Ok(paged),
                errors => Problem(errors));
        }
    }
}
