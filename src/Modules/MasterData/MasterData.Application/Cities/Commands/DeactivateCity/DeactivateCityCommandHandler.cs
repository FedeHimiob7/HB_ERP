using ErrorOr;
using MasterData.Application.Interfaces;
using MasterData.Domain.DomainErrors;
using MasterData.Domain.Repositories;
using MasterData.Domain.VO;
using MediatR;

namespace MasterData.Application.Cities.Commands.DeactivateCity
{
    internal sealed class DeactivateCityCommandHandler : IRequestHandler<DeactivateCityCommand, ErrorOr<Success>>
    {
        private readonly ICityRepository _cityRepository;
        private readonly IMasterDataUnitOfWork _unitOfWork;

        public DeactivateCityCommandHandler(ICityRepository cityRepository, IMasterDataUnitOfWork unitOfWork)
        {
            _cityRepository = cityRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Success>> Handle(DeactivateCityCommand request, CancellationToken cancellationToken)
        {
            var city = await _cityRepository.GetByIdAsync(CityId.Create(request.Id), cancellationToken);
            if (city is null)
                return CityErrors.NotFound;

            city.Deactivate();

            await _cityRepository.UpdateAsync(city, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success;
        }
    }
}
