using ErrorOr;
using MasterData.Application.States.Models;
using MasterData.Domain.DomainErrors;
using MasterData.Domain.Repositories;
using MasterData.Domain.VO;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterData.Application.States.Queries.GetById
{
    internal sealed class GetStateByIdQueryHandler : IRequestHandler<GetStateByIdQuery, ErrorOr<StateResponse>>
    {
        private readonly IStateRepository _repository;

        public GetStateByIdQueryHandler(IStateRepository repository) => _repository = repository;

        public async Task<ErrorOr<StateResponse>> Handle(GetStateByIdQuery request, CancellationToken cancellationToken)
        {
            var state = await _repository.GetByIdAsync(StateId.Create(request.Id), cancellationToken);

            if (state is null) return StateErrors.NotFound;

            return new StateResponse(
                state.Id.Value,
                state.CountryId.Value,
                state.Code,
                state.Name);
        }
    }
}
