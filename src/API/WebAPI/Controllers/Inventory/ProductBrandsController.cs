using Inventory.Application.ProductBrands.Commands.CreateProductBrand;
using Inventory.Application.ProductBrands.Commands.DeactivateProductBrand;
using Inventory.Application.ProductBrands.Commands.UpdateProductBrand;
using Inventory.Application.ProductBrands.Queries.GetAll;
using Inventory.Application.ProductBrands.Queries.GetById;
using Inventory.Application.ProductBrands.Queries.GetPaged;
using Inventory.Domain.SearchParametersModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPI.APIModels.Inventory.ProductBrand;

namespace WebAPI.Controllers.Inventory
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class ProductBrandsController : ApiController
    {
        private readonly ISender _sender;

        public ProductBrandsController(ISender sender)
        {
            _sender = sender;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProductBrandRequest request, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(new CreateProductBrandCommand(request.Name, request.Description), cancellationToken);
            return result.Match(
                id => CreatedAtAction(nameof(GetById), new { id }, new { Id = id }),
                errors => Problem(errors));
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProductBrandRequest request, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(new UpdateProductBrandCommand(id, request.Name, request.Description), cancellationToken);
            return result.Match(
                updated => Ok(updated),
                errors => Problem(errors));
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Deactivate(Guid id, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(new DeactivateProductBrandCommand(id), cancellationToken);
            return result.Match(
                _ => NoContent(),
                errors => Problem(errors));
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(new GetProductBrandByIdQuery(id), cancellationToken);
            return result.Match(
                item => Ok(item),
                errors => Problem(errors));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var result = await _sender.Send(new GetAllProductBrandsQuery(), cancellationToken);
            return result.Match(
                items => Ok(items),
                errors => Problem(errors));
        }

        [HttpGet("paged")]
        public async Task<IActionResult> GetPaged([FromQuery] GetProductBrandsPagedRequest request, CancellationToken cancellationToken)
        {
            var filter = new ProductBrandFilter(request.PageNumber, request.PageSize, request.SearchTerm);
            var result = await _sender.Send(new GetProductBrandsPagedQuery(filter), cancellationToken);
            return result.Match(
                paged => Ok(paged),
                errors => Problem(errors));
        }
    }
}
