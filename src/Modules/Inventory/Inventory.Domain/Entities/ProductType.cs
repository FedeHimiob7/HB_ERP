using ErrorOr;
using HB_ERP.SharedKernel.Domain.Primitives;
using Inventory.Domain.DomainErrors;
using Inventory.Domain.Events;
using Inventory.Domain.VO;

namespace Inventory.Domain.Entities
{
    public sealed class ProductType : AggregateRoot<ProductTypeId>
    {
        private ProductType() { }

        private ProductType(ProductTypeId id, string name, string? description, bool isActive)
            : base(id)
        {
            Name = name;
            Description = description;
            IsActive = isActive;
        }

        public string Name { get; private set; } = string.Empty;
        public string? Description { get; private set; }
        public bool IsActive { get; private set; }

        public static ErrorOr<ProductType> Create(string name, string? description)
        {
            if (string.IsNullOrWhiteSpace(name))
                return ProductTypeErrors.NameIsRequired;

            var productType = new ProductType(ProductTypeId.New(), name.Trim(), description?.Trim(), isActive: true);
            productType.Raise(new ProductTypeCreatedDomainEvent(productType.Id, productType.Name));
            return productType;
        }

        public static ProductType CreateExisting(Guid id, string name, string? description, bool isActive)
            => new ProductType(ProductTypeId.Create(id), name, description, isActive);

        public ErrorOr<Success> UpdateDetails(string name, string? description)
        {
            if (string.IsNullOrWhiteSpace(name))
                return ProductTypeErrors.NameIsRequired;

            Name = name.Trim();
            Description = description?.Trim();
            return Result.Success;
        }

        public void Activate() { if (!IsActive) IsActive = true; }
        public void Deactivate() { if (IsActive) IsActive = false; }
    }
}
