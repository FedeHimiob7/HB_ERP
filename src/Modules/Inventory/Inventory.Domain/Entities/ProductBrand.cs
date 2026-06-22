using ErrorOr;
using HB_ERP.SharedKernel.Domain.Primitives;
using Inventory.Domain.DomainErrors;
using Inventory.Domain.Events;
using Inventory.Domain.VO;

namespace Inventory.Domain.Entities
{
    public sealed class ProductBrand : AggregateRoot<ProductBrandId>
    {
        private ProductBrand() { }

        private ProductBrand(ProductBrandId id, string name, string? description, bool isActive)
            : base(id)
        {
            Name = name;
            Description = description;
            IsActive = isActive;
        }

        public string Name { get; private set; } = string.Empty;
        public string? Description { get; private set; }
        public bool IsActive { get; private set; }

        public static ErrorOr<ProductBrand> Create(string name, string? description)
        {
            if (string.IsNullOrWhiteSpace(name))
                return ProductBrandErrors.NameIsRequired;

            var brand = new ProductBrand(ProductBrandId.New(), name.Trim(), description?.Trim(), isActive: true);
            brand.Raise(new ProductBrandCreatedDomainEvent(brand.Id, brand.Name));
            return brand;
        }

        public static ProductBrand CreateExisting(Guid id, string name, string? description, bool isActive)
            => new ProductBrand(ProductBrandId.Create(id), name, description, isActive);

        public ErrorOr<Success> UpdateDetails(string name, string? description)
        {
            if (string.IsNullOrWhiteSpace(name))
                return ProductBrandErrors.NameIsRequired;

            Name = name.Trim();
            Description = description?.Trim();
            return Result.Success;
        }

        public void Activate() { if (!IsActive) IsActive = true; }
        public void Deactivate() { if (IsActive) IsActive = false; }
    }
}
