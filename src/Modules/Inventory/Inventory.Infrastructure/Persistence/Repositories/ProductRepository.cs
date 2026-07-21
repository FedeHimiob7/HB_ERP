using Inventory.Domain.Entities;
using Inventory.Domain.Repositories;
using Inventory.Domain.SearchParametersModel;
using Inventory.Domain.VO;
using MasterData.Domain.VO;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Inventory.Infrastructure.Persistence.Repositories
{
    internal sealed class ProductRepository : IProductRepository
    {
        private readonly InventoryDbContext _dbContext;

        public ProductRepository(InventoryDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Product?> GetByIdAsync(ProductId id, IReadOnlyList<Guid> allowedPslIds, CancellationToken cancellationToken = default)
        {
            var pslVoList = allowedPslIds.Select(g => new ProductServiceLineId(g)).ToList();
            var product = await _dbContext.Products
                .FirstOrDefaultAsync(x => x.Id == id && pslVoList.Contains(x.ProductServiceLineId), cancellationToken);

            if (product is not null)
                LoadTaxIds(product);

            return product;
        }

        public async Task<List<Product>> GetAllAsync(IReadOnlyList<Guid> allowedPslIds, ProductServiceLineId? productServiceLineId = null, CancellationToken cancellationToken = default)
        {
            var pslVoList = allowedPslIds.Select(g => new ProductServiceLineId(g)).ToList();
            var query = _dbContext.Products
                .Where(x => pslVoList.Contains(x.ProductServiceLineId));

            if (productServiceLineId.HasValue)
                query = query.Where(x => x.ProductServiceLineId == productServiceLineId.Value);

            var products = await query.OrderBy(x => x.Name).ToListAsync(cancellationToken);
            products.ForEach(LoadTaxIds);
            return products;
        }

        public async Task<(IReadOnlyList<Product> Items, int TotalCount)> GetPagedAsync(
            ProductFilter filter, IReadOnlyList<Guid> allowedPslIds, CancellationToken cancellationToken = default)
        {
            var pslVoList = allowedPslIds.Select(g => new ProductServiceLineId(g)).ToList();
            var query = _dbContext.Products
                .Where(x => pslVoList.Contains(x.ProductServiceLineId));

            if (filter.ProductServiceLineId.HasValue)
                query = query.Where(x => x.ProductServiceLineId == new ProductServiceLineId(filter.ProductServiceLineId.Value));

            if (filter.ProductTypeId.HasValue)
                query = query.Where(x => x.ProductTypeId == new ProductTypeId(filter.ProductTypeId.Value));

            if (filter.ProductCategoryId.HasValue)
                query = query.Where(x => x.ProductCategoryId == new ProductCategoryId(filter.ProductCategoryId.Value));

            if (filter.ProductBrandId.HasValue)
                query = query.Where(x => x.ProductBrandId == new ProductBrandId(filter.ProductBrandId.Value));

            if (filter.IsSalable.HasValue)
                query = query.Where(x => x.IsSalable == filter.IsSalable.Value);

            if (filter.IsPurchasable.HasValue)
                query = query.Where(x => x.IsPurchasable == filter.IsPurchasable.Value);

            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                var term = filter.SearchTerm.ToLower();
                query = query.Where(x =>
                    x.Name.ToLower().Contains(term) ||
                    x.Code.ToLower().Contains(term) ||
                    (x.Barcode != null && x.Barcode.ToLower().Contains(term)) ||
                    (x.ClientCode != null && x.ClientCode.ToLower().Contains(term)));
            }

            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query
                .OrderBy(x => x.Name)
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync(cancellationToken);

            items.ForEach(LoadTaxIds);
            return (items, totalCount);
        }

        public async Task AddAsync(Product product, CancellationToken cancellationToken = default)
        {
            PersistTaxIds(product);
            await _dbContext.Products.AddAsync(product, cancellationToken);
        }

        public Task UpdateAsync(Product product, CancellationToken cancellationToken = default)
        {
            PersistTaxIds(product);
            _dbContext.Products.Update(product);
            return Task.CompletedTask;
        }

        public async Task<bool> ExistsByCodeAsync(string code, ProductId? excludeId = null, CancellationToken cancellationToken = default)
        {
            var query = _dbContext.Products.AsNoTracking()
                .Where(x => x.Code.ToLower() == code.ToLower());
            if (excludeId.HasValue)
                query = query.Where(x => x.Id != excludeId.Value);
            return await query.AnyAsync(cancellationToken);
        }

        public async Task<ProductPriceHistory?> GetLastPriceHistoryAsync(ProductId id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Set<ProductPriceHistory>()
                .AsNoTracking()
                .Where(h => h.ProductId == id)
                .OrderByDescending(h => h.ChangedAt)
                .FirstOrDefaultAsync(cancellationToken);
        }

        // ── Shadow property helpers ─────────────────────────────────────────────

        private void LoadTaxIds(Product product)
        {
            var entry = _dbContext.Entry(product);
            var purchaseJson = entry.Property<string>("PurchaseTaxIdsJson").CurrentValue ?? "[]";
            var saleJson = entry.Property<string>("SaleTaxIdsJson").CurrentValue ?? "[]";

            var purchaseIds = JsonSerializer.Deserialize<List<Guid>>(purchaseJson)!
                .Select(g => TaxId.Create(g));
            var saleIds = JsonSerializer.Deserialize<List<Guid>>(saleJson)!
                .Select(g => TaxId.Create(g));

            product.SetTaxes(purchaseIds, saleIds);

            // Reset shadow properties to original values so EF doesn't consider them modified
            entry.Property<string>("PurchaseTaxIdsJson").OriginalValue = purchaseJson;
            entry.Property<string>("SaleTaxIdsJson").OriginalValue = saleJson;
        }

        private void PersistTaxIds(Product product)
        {
            var entry = _dbContext.Entry(product);
            entry.Property<string>("PurchaseTaxIdsJson").CurrentValue =
                JsonSerializer.Serialize(product.PurchaseTaxIds.Select(t => t.Value).ToList());
            entry.Property<string>("SaleTaxIdsJson").CurrentValue =
                JsonSerializer.Serialize(product.SaleTaxIds.Select(t => t.Value).ToList());
        }
    }
}
