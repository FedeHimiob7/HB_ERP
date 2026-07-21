using Inventory.Application.Products.Commands.CreateProduct;
using Inventory.Application.Products.Commands.DeactivateProduct;
using Inventory.Application.Products.Commands.GenerateProductCode;
using Inventory.Application.Products.Commands.UpdateProduct;
using Inventory.Application.Products.Commands.UpdateProductPrices;
using Inventory.Application.Products.Queries.CalculatePrices;
using Inventory.Application.Products.Queries.GetAll;
using Inventory.Application.Products.Queries.GetById;
using Inventory.Application.Products.Queries.GetLastPrice;
using Inventory.Application.Products.Queries.GetPaged;
using Inventory.Domain.SearchParametersModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPI.APIModels.Inventory.Product;
using WebAPI.APIModels.Inventory.Product.Mapper;

namespace WebAPI.Controllers.Inventory
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class ProductsController : ApiController
    {
        private readonly ISender _sender;

        public ProductsController(ISender sender)
        {
            _sender = sender;
        }

        [HttpPost("calculate-prices")]
        public async Task<IActionResult> CalculatePrices([FromBody] CalculatePricesRequest request, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(request.ToCalculatePricesQuery(), cancellationToken);
            return result.Match(
                prices => Ok(prices),
                errors => Problem(errors));
        }

        [HttpPost("generate-code")]
        public async Task<IActionResult> GenerateCode([FromQuery] Guid productServiceLineId, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(new GenerateProductCodeCommand(productServiceLineId), cancellationToken);
            return result.Match(
                code => Ok(code),
                errors => Problem(errors));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProductRequest request, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(request.ToCreateCommand(), cancellationToken);
            return result.Match(
                id => CreatedAtAction(nameof(GetById), new { id }, new { Id = id }),
                errors => Problem(errors));
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProductRequest request, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(request.ToUpdateCommand(id), cancellationToken);
            return result.Match(
                updated => Ok(updated),
                errors => Problem(errors));
        }

        [HttpPut("{id:guid}/prices")]
        public async Task<IActionResult> UpdatePrices(Guid id, [FromBody] UpdateProductPricesRequest request, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(request.ToUpdatePricesCommand(id), cancellationToken);
            return result.Match(
                updated => Ok(updated),
                errors => Problem(errors));
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Deactivate(Guid id, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(new DeactivateProductCommand(id), cancellationToken);
            return result.Match(
                _ => NoContent(),
                errors => Problem(errors));
        }

        [HttpGet("{id:guid}/last-price")]
        public async Task<IActionResult> GetLastPrice(Guid id, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(new GetProductLastPriceQuery(id), cancellationToken);
            return result.Match(
                lastPrice => Ok(lastPrice),
                errors => Problem(errors));
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(new GetProductByIdQuery(id), cancellationToken);
            return result.Match(
                item => Ok(item),
                errors => Problem(errors));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] Guid? productServiceLineId = null, CancellationToken cancellationToken = default)
        {
            var result = await _sender.Send(new GetAllProductsQuery(productServiceLineId), cancellationToken);
            return result.Match(
                items => Ok(items),
                errors => Problem(errors));
        }

        [HttpGet("paged")]
        public async Task<IActionResult> GetPaged([FromQuery] GetProductsPagedRequest request, CancellationToken cancellationToken)
        {
            var filter = new ProductFilter(
                request.PageNumber,
                request.PageSize,
                request.SearchTerm,
                request.ProductServiceLineId,
                request.ProductTypeId,
                request.ProductCategoryId,
                request.ProductBrandId,
                request.IsSalable,
                request.IsPurchasable);

            var result = await _sender.Send(new GetProductsPagedQuery(filter), cancellationToken);
            return result.Match(
                paged => Ok(paged),
                errors => Problem(errors));
        }
    }
}
