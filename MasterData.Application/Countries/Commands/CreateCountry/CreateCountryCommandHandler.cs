using ErrorOr;
using HB_ERP.SharedKernel.Application.Interfaces;
using MasterData.Domain.Entities;
using MasterData.Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterData.Application.Countries.Commands.CreateCountry
{
    internal sealed class CreateCountryCommandHandler
    : IRequestHandler<CreateCountryCommand, ErrorOr<Guid>>
    {
        private readonly ICountryRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateCountryCommandHandler(
            ICountryRepository repository,
            IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Guid>> Handle(CreateCountryCommand request, CancellationToken cancellationToken)
        {
            var createResult = Country.Create(request.Name);

            if (createResult.IsError)
                return createResult.Errors;

            var country = createResult.Value;

            await _repository.AddAsync(country, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return country.Id.Value;
        }
    }
}
