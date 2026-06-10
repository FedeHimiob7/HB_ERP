using ErrorOr;
using MasterData.Application.ProductServiceLines.Models;
using MasterData.Domain.Repositories;
using MediatR;

namespace MasterData.Application.ProductServiceLines.Queries.GetAll
{
    internal sealed class GetAllProductServiceLinesQueryHandler
        : IRequestHandler<GetAllProductServiceLinesQuery, ErrorOr<IReadOnlyList<ProductServiceLineResponse>>>
    {
        private readonly IProductServiceLineRepository _repository;

        public GetAllProductServiceLinesQueryHandler(IProductServiceLineRepository repository)
            => _repository = repository;

        public async Task<ErrorOr<IReadOnlyList<ProductServiceLineResponse>>> Handle(
            GetAllProductServiceLinesQuery request,
            CancellationToken cancellationToken)
        {
            var lines = await _repository.GetAllAsync(cancellationToken);

            var response = lines.Select(psl => new ProductServiceLineResponse(
                psl.Id.Value,
                psl.Description,
                psl.Name
            )).ToList();

            return response;
        }
    }
}
