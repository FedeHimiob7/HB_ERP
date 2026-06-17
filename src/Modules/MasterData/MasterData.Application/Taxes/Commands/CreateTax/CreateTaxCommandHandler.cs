using ErrorOr;
using MasterData.Application.Interfaces;
using MasterData.Domain.Entities;
using MasterData.Domain.Repositories;
using MediatR;

namespace MasterData.Application.Taxes.Commands.CreateTax
{
    internal sealed class CreateTaxCommandHandler : IRequestHandler<CreateTaxCommand, ErrorOr<Guid>>
    {
        private readonly ITaxRepository _repository;
        private readonly IMasterDataUnitOfWork _unitOfWork;

        public CreateTaxCommandHandler(ITaxRepository repository, IMasterDataUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Guid>> Handle(CreateTaxCommand request, CancellationToken cancellationToken)
        {
            var createResult = Tax.Create(request.Name, request.TaxType, request.Rate);
            if (createResult.IsError) return createResult.Errors;

            await _repository.AddAsync(createResult.Value, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return createResult.Value.Id.Value;
        }
    }
}
