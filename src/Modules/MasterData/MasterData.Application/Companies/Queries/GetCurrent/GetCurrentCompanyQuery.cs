using ErrorOr;
using MasterData.Application.Companies.Models;
using MediatR;

namespace MasterData.Application.Companies.Queries.GetCurrent
{
    public record GetCurrentCompanyQuery : IRequest<ErrorOr<CompanyResponse>>;
}
