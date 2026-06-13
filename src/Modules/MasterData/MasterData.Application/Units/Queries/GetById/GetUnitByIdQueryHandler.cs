using ErrorOr;
using MasterData.Application.Units.Models;
using MasterData.Domain.DomainErrors;
using MasterData.Domain.Repositories;
using MasterData.Domain.VO;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterData.Application.Units.Queries.GetById
{
    internal sealed class GetUnitByIdQueryHandler : IRequestHandler<GetUnitByIdQuery, ErrorOr<UnitResponse>>
    {
        private readonly IUnitRepository _repository;
        public GetUnitByIdQueryHandler(IUnitRepository repository) => _repository = repository;

        public async Task<ErrorOr<UnitResponse>> Handle(GetUnitByIdQuery request, CancellationToken cancellationToken)
        {
            var unit = await _repository.GetByIdAsync(UnitId.Create(request.Id), cancellationToken);
            if (unit is null) return UnitErrors.NotFound;

            return new UnitResponse(unit.Id.Value, unit.Name, unit.Description);
        }
    }
}
