using ErrorOr;
using HB_ERP.SharedKernel.Application.Interfaces;
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

namespace MasterData.Application.States.Commands.UpdateState
{
    internal sealed class UpdateStateCommandHandler
    : IRequestHandler<UpdateStateCommand, ErrorOr<StateResponse>>
    {
        private readonly IStateRepository _stateRepository;
        private readonly ICountryRepository _countryRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateStateCommandHandler(
            IStateRepository stateRepository,
            ICountryRepository countryRepository,
            IUnitOfWork unitOfWork)
        {
            _stateRepository = stateRepository;
            _countryRepository = countryRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<StateResponse>> Handle(UpdateStateCommand request, CancellationToken cancellationToken)
        {
            var stateId = StateId.Create(request.Id);
            var countryId = CountryId.Create(request.CountryId);

            var state = await _stateRepository.GetByIdAsync(stateId, cancellationToken);
            if (state is null)
                return StateErrors.NotFound;

            if (state.CountryId != countryId)
            {
                var countryExists = await _countryRepository.GetByIdAsync(countryId, cancellationToken);
                if (countryExists is null)
                    return StateErrors.InvalidCountry;
            }

            var updateResult = state.UpdateDetails(countryId, request.Code, request.Name);
            if (updateResult.IsError)
                return updateResult.Errors;

            await _stateRepository.UpdateAsync(state, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new StateResponse(
                state.Id.Value,
                state.CountryId.Value,
                state.Code,
                state.Name);
        }
    }
}
