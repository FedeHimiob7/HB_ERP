using Inventory.Application.StorageTypes.Commands.CreateStorageType;
using Inventory.Application.StorageTypes.Commands.DeactivateStorageType;
using Inventory.Application.StorageTypes.Commands.UpdateStorageType;
using Inventory.Application.StorageTypes.Queries.GetAll;
using Inventory.Application.StorageTypes.Queries.GetById;
using Inventory.Application.StorageTypes.Queries.GetPaged;
using Inventory.Domain.SearchParametersModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPI.APIModels.Inventory.StorageType;

namespace WebAPI.Controllers.Inventory
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class StorageTypesController : ApiController
    {
        private readonly ISender _sender;

        public StorageTypesController(ISender sender)
        {
            _sender = sender;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateStorageTypeRequest request, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(new CreateStorageTypeCommand(request.Name, request.Description), cancellationToken);
            return result.Match(
                id => CreatedAtAction(nameof(GetById), new { id }, new { Id = id }),
                errors => Problem(errors));
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateStorageTypeRequest request, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(new UpdateStorageTypeCommand(id, request.Name, request.Description), cancellationToken);
            return result.Match(
                updated => Ok(updated),
                errors => Problem(errors));
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Deactivate(Guid id, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(new DeactivateStorageTypeCommand(id), cancellationToken);
            return result.Match(
                _ => NoContent(),
                errors => Problem(errors));
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(new GetStorageTypeByIdQuery(id), cancellationToken);
            return result.Match(
                item => Ok(item),
                errors => Problem(errors));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var result = await _sender.Send(new GetAllStorageTypesQuery(), cancellationToken);
            return result.Match(
                items => Ok(items),
                errors => Problem(errors));
        }

        [HttpGet("paged")]
        public async Task<IActionResult> GetPaged([FromQuery] GetStorageTypesPagedRequest request, CancellationToken cancellationToken)
        {
            var filter = new StorageTypeFilter(request.PageNumber, request.PageSize, request.SearchTerm);
            var result = await _sender.Send(new GetStorageTypesPagedQuery(filter), cancellationToken);
            return result.Match(
                paged => Ok(paged),
                errors => Problem(errors));
        }
    }
}
