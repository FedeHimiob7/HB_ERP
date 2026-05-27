using ErrorOr;
using HB_ERP.SharedKernel.Application.Interfaces;
using MasterData.Application.Countries.Models;
using MasterData.Domain.Repositories;
using MasterData.Domain.VO;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterData.Application.Countries.Commands.UpdateCountry
{
    internal sealed class UpdateCountryCommandHandler
    : IRequestHandler<UpdateCountryCommand, ErrorOr<CountryResponse>>
    {
        private readonly ICountryRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateCountryCommandHandler(
            ICountryRepository repository,
            IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<CountryResponse>> Handle(UpdateCountryCommand request, CancellationToken cancellationToken)
        {
            var countryId = CountryId.Create(request.Id);
            var country = await _repository.GetByIdAsync(countryId, cancellationToken);

            if (country is null)
            {
                return Error.NotFound(
                    code: "Country.NotFound",
                    description: "El país solicitado no existe.");
            }

            var updateResult = country.UpdateDetails(request.Name);

            if (updateResult.IsError)
                return updateResult.Errors;

            await _repository.UpdateAsync(country, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new CountryResponse(
                country.Id.Value,
                country.Name);
        }
    }
}
