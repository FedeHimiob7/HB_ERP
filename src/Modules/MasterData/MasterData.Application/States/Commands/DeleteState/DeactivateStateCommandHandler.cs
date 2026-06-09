using ErrorOr;
using MasterData.Application.Interfaces;
using MasterData.Domain.DomainErrors;
using MasterData.Domain.Repositories;
using MasterData.Domain.VO;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterData.Application.States.Commands.DeleteState
{
    internal sealed class DeactivateStateCommandHandler : IRequestHandler<DeactivateStateCommand, ErrorOr<Success>>
    {
        private readonly IStateRepository _repository;
        private readonly IMasterDataUnitOfWork _unitOfWork;

        public DeactivateStateCommandHandler(IStateRepository repository, IMasterDataUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Success>> Handle(DeactivateStateCommand request, CancellationToken cancellationToken)
        {
            var state = await _repository.GetByIdAsync(StateId.Create(request.Id), cancellationToken);

            if (state is null) return StateErrors.NotFound;

            state.Deactivate();

            await _repository.UpdateAsync(state, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success;
        }
    }
}
