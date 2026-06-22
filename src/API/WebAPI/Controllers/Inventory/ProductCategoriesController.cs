using Inventory.Application.ProductCategories.Commands.CreateProductCategory;
using Inventory.Application.ProductCategories.Commands.DeactivateProductCategory;
using Inventory.Application.ProductCategories.Commands.UpdateProductCategory;
using Inventory.Application.ProductCategories.Queries.GetAll;
using Inventory.Application.ProductCategories.Queries.GetById;
using Inventory.Application.ProductCategories.Queries.GetPaged;
using Inventory.Domain.SearchParametersModel;
using Inventory.Domain.VO;
using MasterData.Domain.VO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPI.APIModels.Inventory.ProductCategory;

namespace WebAPI.Controllers.Inventory
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class ProductCategoriesController : ApiController
    {
        private readonly ISender _sender;

        public ProductCategoriesController(ISender sender)
        {
            _sender = sender;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProductCategoryRequest request, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(
                new CreateProductCategoryCommand(request.ProductServiceLineId, request.ProductTypeId, request.Name, request.Description),
                cancellationToken);
            return result.Match(
                id => CreatedAtAction(nameof(GetById), new { id }, new { Id = id }),
                errors => Problem(errors));
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProductCategoryRequest request, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(
                new UpdateProductCategoryCommand(id, request.ProductServiceLineId, request.ProductTypeId, request.Name, request.Description),
                cancellationToken);
            return result.Match(
                updated => Ok(updated),
                errors => Problem(errors));
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Deactivate(Guid id, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(new DeactivateProductCategoryCommand(id), cancellationToken);
            return result.Match(
                _ => NoContent(),
                errors => Problem(errors));
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(new GetProductCategoryByIdQuery(id), cancellationToken);
            return result.Match(
                item => Ok(item),
                errors => Problem(errors));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] Guid? productServiceLineId = null, CancellationToken cancellationToken = default)
        {
            var result = await _sender.Send(new GetAllProductCategoriesQuery(productServiceLineId), cancellationToken);
            return result.Match(
                items => Ok(items),
                errors => Problem(errors));
        }

        [HttpGet("paged")]
        public async Task<IActionResult> GetPaged([FromQuery] GetProductCategoriesPagedRequest request, CancellationToken cancellationToken)
        {
            var pslId = request.ProductServiceLineId.HasValue
                ? ProductServiceLineId.Create(request.ProductServiceLineId.Value)
                : (ProductServiceLineId?)null;

            var typeId = request.ProductTypeId.HasValue
                ? new ProductTypeId(request.ProductTypeId.Value)
                : (ProductTypeId?)null;

            var filter = new ProductCategoryFilter(request.PageNumber, request.PageSize, request.SearchTerm, pslId, typeId);
            var result = await _sender.Send(new GetProductCategoriesPagedQuery(filter), cancellationToken);
            return result.Match(
                paged => Ok(paged),
                errors => Problem(errors));
        }
    }
}
