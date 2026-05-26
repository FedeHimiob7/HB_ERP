using ErrorOr;
using HB_ERP.SharedKernel.Application.Interfaces;
using MasterData.Domain.Repositories;
using MasterData.Domain.VO;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterData.Application.ProductServiceLines.Commands.DesactivatePSL
{
    internal sealed class DeactivateProductServiceLineCommandHandler
    : IRequestHandler<DeactivateProductServiceLineCommand, ErrorOr<Success>>
    {
        private readonly IProductServiceLineRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public DeactivateProductServiceLineCommandHandler(
            IProductServiceLineRepository repository,
            IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Success>> Handle(DeactivateProductServiceLineCommand request, CancellationToken cancellationToken)
        {
            var productServiceLineId = ProductServiceLineId.Create(request.Id);

            var productServiceLine = await _repository.GetByIdAsync(productServiceLineId, cancellationToken);

            if (productServiceLine is null)
            {
                return Error.NotFound(
                    code: "ProductServiceLine.NotFound",
                    description: "La línea de servicio de producto solicitada no existe.");
            }

            productServiceLine.Deactivate();

            await _repository.UpdateAsync(productServiceLine, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success;
        }
    }
}
