using ErrorOr;
using HB_ERP.SharedKernel.Domain.Primitives;
using MasterData.Application.ProductServiceLines.Models;
using MasterData.Domain.Repositories;
using MediatR;

namespace MasterData.Application.ProductServiceLines.Queries.GetAll
{
    internal sealed class GetAllProductServiceLinesQueryHandler
        : IRequestHandler<GetAllProductServiceLinesQuery, ErrorOr<IReadOnlyList<ProductServiceLineResponse>>>
    {
        private readonly IProductServiceLineRepository _repository;
        private readonly ICurrentUserProvider _currentUser;

        public GetAllProductServiceLinesQueryHandler(IProductServiceLineRepository repository, ICurrentUserProvider currentUser)
        {
            _repository = repository;
            _currentUser = currentUser;
        }

        public async Task<ErrorOr<IReadOnlyList<ProductServiceLineResponse>>> Handle(
            GetAllProductServiceLinesQuery request,
            CancellationToken cancellationToken)
        {
            var lines = await _repository.GetAllAsync(_currentUser.PslIds, cancellationToken);

            var response = lines.Select(psl => new ProductServiceLineResponse(
                psl.Id.Value,
                psl.Description,
                psl.Name
            )).ToList();

            return response;
        }
    }
}
