using ErrorOr;
using MasterData.Application.Interfaces;
using MasterData.Application.Taxes.Models;
using MasterData.Domain.DomainErrors;
using MasterData.Domain.Repositories;
using MasterData.Domain.VO;
using MediatR;

namespace MasterData.Application.Taxes.Commands.UpdateTax
{
    internal sealed class UpdateTaxCommandHandler : IRequestHandler<UpdateTaxCommand, ErrorOr<TaxResponse>>
    {
        private readonly ITaxRepository _repository;
        private readonly IMasterDataUnitOfWork _unitOfWork;

        public UpdateTaxCommandHandler(ITaxRepository repository, IMasterDataUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<TaxResponse>> Handle(UpdateTaxCommand request, CancellationToken cancellationToken)
        {
            var tax = await _repository.GetByIdAsync(TaxId.Create(request.Id), cancellationToken);
            if (tax is null) return TaxErrors.NotFound;

            var updateResult = tax.UpdateDetails(request.Name, request.TaxType, request.Rate);
            if (updateResult.IsError) return updateResult.Errors;

            await _repository.UpdateAsync(tax, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new TaxResponse(tax.Id.Value, tax.Name, tax.TaxType, tax.TaxType.ToString(), tax.Rate);
        }
    }
}
