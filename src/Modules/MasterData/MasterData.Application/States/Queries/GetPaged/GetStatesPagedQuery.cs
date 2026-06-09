using ErrorOr;
using MasterData.Application.States.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterData.Application.States.Queries.GetPaged
{
    public record GetStatesPagedQuery(
    int PageNumber,
    int PageSize,
    Guid? CountryId = null,
    string? SearchTerm = null
) : IRequest<ErrorOr<PagedStatesResult>>;
}
