using ErrorOr;
using MasterData.Application.Interfaces;
using MasterData.Domain.DomainErrors;
using MasterData.Domain.Repositories;
using MasterData.Domain.VO;
using MediatR;

namespace MasterData.Application.Taxes.Commands.DeactivateTax
{
    internal sealed class DeactivateTaxCommandHandler : IRequestHandler<DeactivateTaxCommand, ErrorOr<Success>>
    {
        private readonly ITaxRepository _repository;
        private readonly IMasterDataUnitOfWork _unitOfWork;

        public DeactivateTaxCommandHandler(ITaxRepository repository, IMasterDataUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Success>> Handle(DeactivateTaxCommand request, CancellationToken cancellationToken)
        {
            var tax = await _repository.GetByIdAsync(TaxId.Create(request.Id), cancellationToken);
            if (tax is null) return TaxErrors.NotFound;

            tax.Deactivate();

            await _repository.UpdateAsync(tax, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success;
        }
    }
}
