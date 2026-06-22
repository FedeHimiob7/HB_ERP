using Inventory.Application.Warehouses.Commands.CreateWarehouse;
using Inventory.Application.Warehouses.Commands.DeactivateWarehouse;
using Inventory.Application.Warehouses.Commands.UpdateWarehouse;
using Inventory.Application.Warehouses.Queries.GetAll;
using Inventory.Application.Warehouses.Queries.GetById;
using Inventory.Application.Warehouses.Queries.GetPaged;
using Inventory.Domain.SearchParametersModel;
using MasterData.Domain.VO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPI.APIModels.Inventory.Warehouse;

namespace WebAPI.Controllers.Inventory
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class WarehousesController : ApiController
    {
        private readonly ISender _sender;

        public WarehousesController(ISender sender)
        {
            _sender = sender;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateWarehouseRequest request, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(
                new CreateWarehouseCommand(request.ProductServiceLineId, request.Name, request.Description, request.Latitude, request.Longitude),
                cancellationToken);
            return result.Match(
                id => CreatedAtAction(nameof(GetById), new { id }, new { Id = id }),
                errors => Problem(errors));
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateWarehouseRequest request, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(
                new UpdateWarehouseCommand(id, request.ProductServiceLineId, request.Name, request.Description, request.Latitude, request.Longitude),
                cancellationToken);
            return result.Match(
                updated => Ok(updated),
                errors => Problem(errors));
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Deactivate(Guid id, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(new DeactivateWarehouseCommand(id), cancellationToken);
            return result.Match(
                _ => NoContent(),
                errors => Problem(errors));
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(new GetWarehouseByIdQuery(id), cancellationToken);
            return result.Match(
                item => Ok(item),
                errors => Problem(errors));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] Guid? productServiceLineId = null, CancellationToken cancellationToken = default)
        {
            var result = await _sender.Send(new GetAllWarehousesQuery(productServiceLineId), cancellationToken);
            return result.Match(
                items => Ok(items),
                errors => Problem(errors));
        }

        [HttpGet("paged")]
        public async Task<IActionResult> GetPaged([FromQuery] GetWarehousesPagedRequest request, CancellationToken cancellationToken)
        {
            var pslId = request.ProductServiceLineId.HasValue
                ? ProductServiceLineId.Create(request.ProductServiceLineId.Value)
                : (ProductServiceLineId?)null;

            var filter = new WarehouseFilter(request.PageNumber, request.PageSize, request.SearchTerm, pslId);
            var result = await _sender.Send(new GetWarehousesPagedQuery(filter), cancellationToken);
            return result.Match(
                paged => Ok(paged),
                errors => Problem(errors));
        }
    }
}
