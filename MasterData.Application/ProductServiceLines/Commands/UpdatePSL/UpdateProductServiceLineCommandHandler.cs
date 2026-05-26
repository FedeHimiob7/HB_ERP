using ErrorOr;
using HB_ERP.SharedKernel.Application.Interfaces;
using MasterData.Application.Currencies.Models;
using MasterData.Application.ProductServiceLines.Models;
using MasterData.Domain.Entities;
using MasterData.Domain.Repositories;
using MasterData.Domain.VO;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterData.Application.ProductServiceLines.Commands.UpdatePSL
{
    internal sealed class UpdateProductServiceLineCommandHandler
    : IRequestHandler<UpdateProductServiceLineCommand, ErrorOr<ProductServiceLineResponse>>
    {
        private readonly IProductServiceLineRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateProductServiceLineCommandHandler(
            IProductServiceLineRepository repository,
            IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<ProductServiceLineResponse>> Handle(UpdateProductServiceLineCommand request, CancellationToken cancellationToken)
        {
            var productServiceLineId = ProductServiceLineId.Create(request.Id);

            var productServiceLine = await _repository.GetByIdAsync(productServiceLineId, cancellationToken);

            
            if (productServiceLine is null)
            {
                return Error.NotFound(
                    code: "ProductServiceLine.NotFound",
                    description: "La línea de servicio de producto solicitada no existe.");            }

           
            var updateResult = productServiceLine.UpdateDetails(request.Description, request.Name);

            if (updateResult.IsError)
            {
                return updateResult.Errors;
            }

            await _repository.UpdateAsync(productServiceLine, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);


            var finalResponse = new ProductServiceLineResponse(
                productServiceLine.Id.Value,
                productServiceLine.Description,
                productServiceLine.Name
            );

            return finalResponse;
        }
    }
}
