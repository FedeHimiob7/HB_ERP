using ErrorOr;
using MasterData.Application.Units.Models;
using MasterData.Domain.SearchParametersModel;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterData.Application.Units.Queries.GetPaged
{
    public record GetUnitsPagedQuery(UnitFilter Filter) : IRequest<ErrorOr<PagedUnitsResult>>;
}
