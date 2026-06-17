using ErrorOr;
using MasterData.Application.Interfaces;
using MasterData.Domain.DomainErrors;
using MasterData.Domain.Entities;
using MasterData.Domain.Repositories;
using MasterData.Domain.VO;
using MediatR;

namespace MasterData.Application.Cities.Commands.CreateCity
{
    internal sealed class CreateCityCommandHandler : IRequestHandler<CreateCityCommand, ErrorOr<Guid>>
    {
        private readonly ICityRepository _cityRepository;
        private readonly IStateRepository _stateRepository;
        private readonly IMasterDataUnitOfWork _unitOfWork;

        public CreateCityCommandHandler(
            ICityRepository cityRepository,
            IStateRepository stateRepository,
            IMasterDataUnitOfWork unitOfWork)
        {
            _cityRepository = cityRepository;
            _stateRepository = stateRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Guid>> Handle(CreateCityCommand request, CancellationToken cancellationToken)
        {
            var stateId = StateId.Create(request.StateId);

            var state = await _stateRepository.GetByIdAsync(stateId, cancellationToken);
            if (state is null)
                return CityErrors.InvalidState;

            if (await _cityRepository.ExistsByNameInStateAsync(request.Name, stateId, cancellationToken: cancellationToken))
                return CityErrors.DuplicateName;

            var createResult = City.Create(stateId, request.Name);
            if (createResult.IsError)
                return createResult.Errors;

            var city = createResult.Value;

            await _cityRepository.AddAsync(city, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return city.Id.Value;
        }
    }
}
