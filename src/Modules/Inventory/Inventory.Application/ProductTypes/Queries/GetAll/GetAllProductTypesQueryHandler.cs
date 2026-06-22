using ErrorOr;
using Inventory.Application.ProductTypes.Models;
using Inventory.Domain.Repositories;
using MediatR;

namespace Inventory.Application.ProductTypes.Queries.GetAll
{
    internal sealed class GetAllProductTypesQueryHandler : IRequestHandler<GetAllProductTypesQuery, ErrorOr<IReadOnlyList<ProductTypeResponse>>>
    {
        private readonly IProductTypeRepository _repository;
        public GetAllProductTypesQueryHandler(IProductTypeRepository repository) => _repository = repository;

        public async Task<ErrorOr<IReadOnlyList<ProductTypeResponse>>> Handle(GetAllProductTypesQuery request, CancellationToken cancellationToken)
        {
            var items = await _repository.GetAllAsync(cancellationToken);
            return items.Select(x => new ProductTypeResponse(x.Id.Value, x.Name, x.Description)).ToList();
        }
    }
}
