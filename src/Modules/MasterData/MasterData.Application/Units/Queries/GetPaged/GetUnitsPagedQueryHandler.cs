using ErrorOr;
using MasterData.Application.Units.Models;
using MasterData.Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterData.Application.Units.Queries.GetPaged
{
    internal sealed class GetUnitsPagedQueryHandler : IRequestHandler<GetUnitsPagedQuery, ErrorOr<PagedUnitsResult>>
    {
        private readonly IUnitRepository _repository;
        public GetUnitsPagedQueryHandler(IUnitRepository repository) => _repository = repository;

        public async Task<ErrorOr<PagedUnitsResult>> Handle(GetUnitsPagedQuery request, CancellationToken cancellationToken)
        {
            var (units, totalCount) = await _repository.GetPagedAsync(request.Filter, cancellationToken);

            var mappedItems = units.Select(u => new UnitResponse(
                u.Id.Value, u.Name, u.Description
            )).ToList();

            return new PagedUnitsResult(mappedItems, totalCount);
        }
    }
}
