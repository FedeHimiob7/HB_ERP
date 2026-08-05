using MasterData.Application.Companies.Commands.CreateCompany;
using MasterData.Application.Companies.Commands.UpdateCompany;
using MasterData.Application.Companies.Queries.GetCurrent;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPI.APIModels.MasterData.Company;

namespace WebAPI.Controllers.MasterData
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class CompaniesController : ApiController
    {
        private readonly ISender _sender;

        public CompaniesController(ISender sender)
        {
            _sender = sender;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCompanyRequest request, CancellationToken cancellationToken)
        {
            var command = new CreateCompanyCommand(request.Rif, request.LegalName, request.RegisteredAddress, request.TaxpayerType);
            ErrorOr<Guid> result = await _sender.Send(command, cancellationToken);

            return result.Match(
                id => CreatedAtAction(nameof(GetCurrent), null, id),
                errors => Problem(errors)
            );
        }

        [HttpPut("current")]
        public async Task<IActionResult> Update([FromBody] UpdateCompanyRequest request, CancellationToken cancellationToken)
        {
            var command = new UpdateCompanyCommand(request.Rif, request.LegalName, request.RegisteredAddress, request.TaxpayerType);
            var result = await _sender.Send(command, cancellationToken);

            return result.Match(
                companyResponse => Ok(companyResponse),
                errors => Problem(errors)
            );
        }

        [HttpGet("current")]
        public async Task<IActionResult> GetCurrent(CancellationToken cancellationToken)
        {
            var result = await _sender.Send(new GetCurrentCompanyQuery(), cancellationToken);

            return result.Match(
                company => Ok(company),
                errors => Problem(errors)
            );
        }
    }
}
