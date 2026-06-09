using ErrorOr;
using MasterData.Application.States.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterData.Application.States.Queries.GetById
{
    public record GetStateByIdQuery(Guid Id) : IRequest<ErrorOr<StateResponse>>;
}
