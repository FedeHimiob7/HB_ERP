using ErrorOr;
using MasterData.Application.Currencies.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterData.Application.Currencies.Queries.GetCurrencyById
{
    public record GetCurrencyByIdQuery(Guid Id) : IRequest<ErrorOr<CurrencyResponse>>;
}
