using ErrorOr;
using HB_ERP.SharedKernel.Domain.Primitives;
using MasterData.Application.ProductServiceLines.Models;
using MasterData.Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterData.Application.ProductServiceLines.Queries.GetPaged
{
    internal sealed class GetProductServiceLinesPagedQueryHandler
    : IRequestHandler<GetProductServiceLinesPagedQuery, ErrorOr<PagedProductServiceLinesResult>>
    {
        private readonly IProductServiceLineRepository _repository;
        private readonly ICurrentUserProvider _currentUser;

        public GetProductServiceLinesPagedQueryHandler(IProductServiceLineRepository repository, ICurrentUserProvider currentUser)
        {
            _repository = repository;
            _currentUser = currentUser;
        }

        public async Task<ErrorOr<PagedProductServiceLinesResult>> Handle(
            GetProductServiceLinesPagedQuery request,
            CancellationToken cancellationToken)
        {
            var (lines, totalCount) = await _repository.GetPagedAsync(
                request.PageNumber,
                request.PageSize,
                _currentUser.PslIds,
                request.SearchTerm,
                cancellationToken);

            var mappedItems = lines.Select(psl => new ProductServiceLineResponse(
                psl.Id.Value,
                psl.Description,
                psl.Name
            )).ToList();

            return new PagedProductServiceLinesResult(mappedItems, totalCount);
        }
    }
}
