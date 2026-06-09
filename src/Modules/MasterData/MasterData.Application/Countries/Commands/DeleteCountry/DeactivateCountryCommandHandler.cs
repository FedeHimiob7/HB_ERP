using ErrorOr;
using MasterData.Application.Interfaces;
using MasterData.Domain.Repositories;
using MasterData.Domain.VO;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterData.Application.Countries.Commands.DeleteCountry
{
    internal sealed class DeactivateCountryCommandHandler
    : IRequestHandler<DeactivateCountryCommand, ErrorOr<Success>>
    {
        private readonly ICountryRepository _repository;
        private readonly IMasterDataUnitOfWork _unitOfWork;

        public DeactivateCountryCommandHandler(
            ICountryRepository repository,
            IMasterDataUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Success>> Handle(DeactivateCountryCommand request, CancellationToken cancellationToken)
        {
            var countryId = CountryId.Create(request.Id);
            var country = await _repository.GetByIdAsync(countryId, cancellationToken);

            if (country is null)
            {
                return Error.NotFound(
                    code: "Country.NotFound",
                    description: "El país solicitado no existe.");
            }

            country.Deactivate();

            await _repository.UpdateAsync(country, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success;
        }
    }
}
