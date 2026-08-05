using MasterData.Application.Branches.Commands.CreateBranch;
using MasterData.Application.Branches.Commands.DeleteBranch;
using MasterData.Application.Branches.Commands.UpdateBranch;
using MasterData.Application.Branches.Queries.GetAll;
using MasterData.Application.Branches.Queries.GetById;
using MasterData.Application.Branches.Queries.GetPaged;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPI.APIModels.MasterData.Branch;

namespace WebAPI.Controllers.MasterData
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class BranchesController : ApiController
    {
        private readonly ISender _sender;

        public BranchesController(ISender sender)
        {
            _sender = sender;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateBranchRequest request, CancellationToken cancellationToken)
        {
            var command = new CreateBranchCommand(request.Name, request.Address);
            ErrorOr<Guid> result = await _sender.Send(command, cancellationToken);

            return result.Match(
                id => CreatedAtAction(nameof(GetById), new { id }, id),
                errors => Problem(errors)
            );
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateBranchRequest request, CancellationToken cancellationToken)
        {
            var command = new UpdateBranchCommand(id, request.Name, request.Address);
            var result = await _sender.Send(command, cancellationToken);

            return result.Match(
                branchResponse => Ok(branchResponse),
                errors => Problem(errors)
            );
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Deactivate(Guid id, CancellationToken cancellationToken)
        {
            var command = new DeactivateBranchCommand(id);
            ErrorOr<Success> result = await _sender.Send(command, cancellationToken);

            return result.Match(
                _ => NoContent(),
                errors => Problem(errors)
            );
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            var query = new GetBranchByIdQuery(id);
            var result = await _sender.Send(query, cancellationToken);

            return result.Match(
                branch => Ok(branch),
                errors => Problem(errors)
            );
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken = default)
        {
            var result = await _sender.Send(new GetAllBranchesQuery(), cancellationToken);

            return result.Match(
                branches => Ok(branches),
                errors => Problem(errors));
        }

        [HttpGet("paged")]
        public async Task<IActionResult> GetPaged(
                [FromQuery] GetBranchesPagedRequest request,
                CancellationToken cancellationToken = default)
        {
            var query = new GetBranchesPagedQuery(
                request.PageNumber,
                request.PageSize,
                request.SearchTerm
            );

            var result = await _sender.Send(query, cancellationToken);

            return result.Match(
                pagedResult => Ok(pagedResult),
                errors => Problem(errors)
            );
        }
    }
}
