using ErrorOr;
using HB_ERP.SharedKernel.Application.Interfaces;
using MasterData.Domain.Repositories;
using MediatR;
using MasterData.Domain.Entities;

namespace MasterData.Application.ProductServiceLines.Commands.CreatePSL
{
    internal sealed class CreateProductServiceLineCommandHandler
     : IRequestHandler<CreateProductServiceLineCommand, ErrorOr<Guid>>
    {
        private readonly IProductServiceLineRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateProductServiceLineCommandHandler(
            IProductServiceLineRepository repository,
            IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Guid>> Handle(CreateProductServiceLineCommand request, CancellationToken cancellationToken)
        {            
            var createResult = ProductServiceLine.Create(request.Description, request.Name);

            if (createResult.IsError)
                return createResult.Errors;

            var productServiceLine = createResult.Value;

            await _repository.AddAsync(productServiceLine, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return productServiceLine.Id.Value;
        }
    }
}
