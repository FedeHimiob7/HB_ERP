using Inventory.Application.ProductSubCategories.Commands.CreateProductSubCategory;
using Inventory.Application.ProductSubCategories.Commands.DeactivateProductSubCategory;
using Inventory.Application.ProductSubCategories.Commands.UpdateProductSubCategory;
using Inventory.Application.ProductSubCategories.Queries.GetAll;
using Inventory.Application.ProductSubCategories.Queries.GetById;
using Inventory.Application.ProductSubCategories.Queries.GetPaged;
using Inventory.Domain.SearchParametersModel;
using Inventory.Domain.VO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPI.APIModels.Inventory.ProductSubCategory;

namespace WebAPI.Controllers.Inventory
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class ProductSubCategoriesController : ApiController
    {
        private readonly ISender _sender;

        public ProductSubCategoriesController(ISender sender)
        {
            _sender = sender;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProductSubCategoryRequest request, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(
                new CreateProductSubCategoryCommand(request.ProductCategoryId, request.Name, request.Description),
                cancellationToken);
            return result.Match(
                id => CreatedAtAction(nameof(GetById), new { id }, new { Id = id }),
                errors => Problem(errors));
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProductSubCategoryRequest request, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(
                new UpdateProductSubCategoryCommand(id, request.ProductCategoryId, request.Name, request.Description),
                cancellationToken);
            return result.Match(
                updated => Ok(updated),
                errors => Problem(errors));
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Deactivate(Guid id, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(new DeactivateProductSubCategoryCommand(id), cancellationToken);
            return result.Match(
                _ => NoContent(),
                errors => Problem(errors));
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(new GetProductSubCategoryByIdQuery(id), cancellationToken);
            return result.Match(
                item => Ok(item),
                errors => Problem(errors));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] Guid? productCategoryId = null, CancellationToken cancellationToken = default)
        {
            var result = await _sender.Send(new GetAllProductSubCategoriesQuery(productCategoryId), cancellationToken);
            return result.Match(
                items => Ok(items),
                errors => Problem(errors));
        }

        [HttpGet("paged")]
        public async Task<IActionResult> GetPaged([FromQuery] GetProductSubCategoriesPagedRequest request, CancellationToken cancellationToken)
        {
            var categoryId = request.ProductCategoryId.HasValue
                ? new ProductCategoryId(request.ProductCategoryId.Value)
                : (ProductCategoryId?)null;

            var filter = new ProductSubCategoryFilter(request.PageNumber, request.PageSize, request.SearchTerm, categoryId);
            var result = await _sender.Send(new GetProductSubCategoriesPagedQuery(filter), cancellationToken);
            return result.Match(
                paged => Ok(paged),
                errors => Problem(errors));
        }
    }
}
