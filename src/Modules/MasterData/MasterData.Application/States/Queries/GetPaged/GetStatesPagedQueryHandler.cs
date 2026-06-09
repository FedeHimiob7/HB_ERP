using ErrorOr;
using MasterData.Application.States.Models;
using MasterData.Domain.Repositories;
using MasterData.Domain.SearchParametersModel;
using MasterData.Domain.VO;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterData.Application.States.Queries.GetPaged
{
    internal sealed class GetStatesPagedQueryHandler : IRequestHandler<GetStatesPagedQuery, ErrorOr<PagedStatesResult>>
    {
        private readonly IStateRepository _repository;

        public GetStatesPagedQueryHandler(IStateRepository repository) => _repository = repository;

        public async Task<ErrorOr<PagedStatesResult>> Handle(GetStatesPagedQuery request, CancellationToken cancellationToken)
        {
            CountryId? mappedCountryId = request.CountryId.HasValue
                ? CountryId.Create(request.CountryId.Value)
                : null;

            var filter = new StateFilter(
                request.PageNumber,
                request.PageSize,
                request.SearchTerm,
                mappedCountryId
            );

            var (states, totalCount) = await _repository.GetPagedAsync(filter, cancellationToken);

            var mappedItems = states.Select(s => new StateResponse(
                s.Id.Value,
                s.CountryId.Value,
                s.Code,
                s.Name
            )).ToList();

            return new PagedStatesResult(mappedItems, totalCount);
        }
    }
}
