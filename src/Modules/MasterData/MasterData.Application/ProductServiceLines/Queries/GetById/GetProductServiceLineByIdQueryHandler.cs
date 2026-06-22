using ErrorOr;
using HB_ERP.SharedKernel.Domain.Primitives;
using MasterData.Application.ProductServiceLines.Models;
using MasterData.Domain.Repositories;
using MasterData.Domain.VO;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterData.Application.ProductServiceLines.Queries.GetById
{
    internal sealed class GetProductServiceLineByIdQueryHandler
    : IRequestHandler<GetProductServiceLineByIdQuery, ErrorOr<ProductServiceLineResponse>>
    {
        private readonly IProductServiceLineRepository _repository;
        private readonly ICurrentUserProvider _currentUser;

        public GetProductServiceLineByIdQueryHandler(IProductServiceLineRepository repository, ICurrentUserProvider currentUser)
        {
            _repository = repository;
            _currentUser = currentUser;
        }

        public async Task<ErrorOr<ProductServiceLineResponse>> Handle(
            GetProductServiceLineByIdQuery request,
            CancellationToken cancellationToken)
        {
            var id = ProductServiceLineId.Create(request.Id);

            var psl = await _repository.GetByIdAsync(id, cancellationToken);

            if (psl is null || !_currentUser.PslIds.Contains(psl.Id.Value))
            {
                return Error.NotFound(
                    code: "ProductServiceLine.NotFound",
                    description: "La línea de servicio de producto solicitada no existe.");
            }

            return new ProductServiceLineResponse(
                psl.Id.Value,
                psl.Description,
                psl.Name);
        }
    }
}
