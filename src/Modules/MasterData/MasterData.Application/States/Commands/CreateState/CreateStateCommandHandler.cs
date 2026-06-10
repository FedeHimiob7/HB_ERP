using ErrorOr;
using MasterData.Application.Interfaces;
using MasterData.Domain.DomainErrors;
using MasterData.Domain.Entities;
using MasterData.Domain.Repositories;
using MasterData.Domain.VO;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterData.Application.States.Commands.CreateState
{
    internal sealed class CreateStateCommandHandler
    : IRequestHandler<CreateStateCommand, ErrorOr<Guid>>
    {
        private readonly IStateRepository _stateRepository;
        private readonly ICountryRepository _countryRepository; 
        private readonly IMasterDataUnitOfWork _unitOfWork;

        public CreateStateCommandHandler(
            IStateRepository stateRepository,
            ICountryRepository countryRepository,
            IMasterDataUnitOfWork unitOfWork)
        {
            _stateRepository = stateRepository;
            _countryRepository = countryRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Guid>> Handle(CreateStateCommand request, CancellationToken cancellationToken)
        {
            var countryId = CountryId.Create(request.CountryId);

            var country = await _countryRepository.GetByIdAsync(countryId, cancellationToken);
            if (country is null)
                return StateErrors.InvalidCountry;

            var borrar = await _stateRepository.ExistsByCodeAsync(request.Code, cancellationToken);

            if (await _stateRepository.ExistsByCodeAsync(request.Code, cancellationToken))
                return StateErrors.DuplicateCode;

            var createResult = State.Create(countryId, request.Code, request.Name);

            if (createResult.IsError)
                return createResult.Errors;

            var state = createResult.Value;

            await _stateRepository.AddAsync(state, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return state.Id.Value;
        }
    }
}
