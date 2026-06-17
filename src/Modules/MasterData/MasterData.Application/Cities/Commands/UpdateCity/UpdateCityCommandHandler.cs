using ErrorOr;
using MasterData.Application.Cities.Models;
using MasterData.Application.Interfaces;
using MasterData.Domain.DomainErrors;
using MasterData.Domain.Repositories;
using MasterData.Domain.VO;
using MediatR;

namespace MasterData.Application.Cities.Commands.UpdateCity
{
    internal sealed class UpdateCityCommandHandler : IRequestHandler<UpdateCityCommand, ErrorOr<CityResponse>>
    {
        private readonly ICityRepository _cityRepository;
        private readonly IStateRepository _stateRepository;
        private readonly IMasterDataUnitOfWork _unitOfWork;

        public UpdateCityCommandHandler(
            ICityRepository cityRepository,
            IStateRepository stateRepository,
            IMasterDataUnitOfWork unitOfWork)
        {
            _cityRepository = cityRepository;
            _stateRepository = stateRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<CityResponse>> Handle(UpdateCityCommand request, CancellationToken cancellationToken)
        {
            var city = await _cityRepository.GetByIdAsync(CityId.Create(request.Id), cancellationToken);
            if (city is null)
                return CityErrors.NotFound;

            var stateId = StateId.Create(request.StateId);

            var state = await _stateRepository.GetByIdAsync(stateId, cancellationToken);
            if (state is null)
                return CityErrors.InvalidState;

            if (await _cityRepository.ExistsByNameInStateAsync(request.Name, stateId, city.Id, cancellationToken))
                return CityErrors.DuplicateName;

            var updateResult = city.UpdateDetails(stateId, request.Name);
            if (updateResult.IsError)
                return updateResult.Errors;

            await _cityRepository.UpdateAsync(city, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new CityResponse(city.Id.Value, city.StateId.Value, city.Name);
        }
    }
}
