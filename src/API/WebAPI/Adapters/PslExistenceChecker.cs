using Identity.Application.Common.Interfaces;
using MasterData.Domain.Repositories;
using MasterData.Domain.VO;

namespace WebAPI.Adapters
{
    public sealed class PslExistenceChecker : IPslExistenceChecker
    {
        private readonly IProductServiceLineRepository _productServiceLineRepository;

        public PslExistenceChecker(IProductServiceLineRepository productServiceLineRepository)
        {
            _productServiceLineRepository = productServiceLineRepository
                ?? throw new ArgumentNullException(nameof(productServiceLineRepository));
        }

        public async Task<bool> ExistsAsync(Guid pslId, CancellationToken cancellationToken = default)
        {
            var psl = await _productServiceLineRepository.GetByIdAsync(
                ProductServiceLineId.Create(pslId), cancellationToken);

            return psl is not null && psl.IsActive;
        }
    }
}
