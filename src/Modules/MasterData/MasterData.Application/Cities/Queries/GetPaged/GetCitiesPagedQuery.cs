using ErrorOr;
using MasterData.Application.Cities.Models;
using MasterData.Domain.SearchParametersModel;
using MediatR;

namespace MasterData.Application.Cities.Queries.GetPaged
{
    public record GetCitiesPagedQuery(CityFilter Filter) : IRequest<ErrorOr<PagedCitiesResult>>;
}
