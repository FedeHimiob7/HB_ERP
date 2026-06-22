using ErrorOr;
using HB_ERP.SharedKernel.Domain.Primitives;
using Inventory.Domain.DomainErrors;
using Inventory.Domain.Events;
using Inventory.Domain.VO;

namespace Inventory.Domain.Entities
{
    public sealed class ProductSubCategory : AggregateRoot<ProductSubCategoryId>
    {
        private ProductSubCategory() { }

        private ProductSubCategory(
            ProductSubCategoryId id,
            ProductCategoryId productCategoryId,
            string name,
            string? description,
            bool isActive)
            : base(id)
        {
            ProductCategoryId = productCategoryId;
            Name = name;
            Description = description;
            IsActive = isActive;
        }

        public ProductCategoryId ProductCategoryId { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public string? Description { get; private set; }
        public bool IsActive { get; private set; }

        public static ErrorOr<ProductSubCategory> Create(
            ProductCategoryId productCategoryId,
            string name,
            string? description)
        {
            if (productCategoryId.Value == Guid.Empty)
                return ProductSubCategoryErrors.InvalidCategory;

            if (string.IsNullOrWhiteSpace(name))
                return ProductSubCategoryErrors.NameIsRequired;

            var subCategory = new ProductSubCategory(
                ProductSubCategoryId.New(),
                productCategoryId,
                name.Trim(),
                description?.Trim(),
                isActive: true);

            subCategory.Raise(new ProductSubCategoryCreatedDomainEvent(
                subCategory.Id,
                subCategory.ProductCategoryId,
                subCategory.Name));

            return subCategory;
        }

        public static ProductSubCategory CreateExisting(
            Guid id,
            Guid productCategoryId,
            string name,
            string? description,
            bool isActive)
            => new ProductSubCategory(
                ProductSubCategoryId.Create(id),
                ProductCategoryId.Create(productCategoryId),
                name, description, isActive);

        public ErrorOr<Success> UpdateDetails(
            ProductCategoryId productCategoryId,
            string name,
            string? description)
        {
            if (productCategoryId.Value == Guid.Empty)
                return ProductSubCategoryErrors.InvalidCategory;

            if (string.IsNullOrWhiteSpace(name))
                return ProductSubCategoryErrors.NameIsRequired;

            ProductCategoryId = productCategoryId;
            Name = name.Trim();
            Description = description?.Trim();
            return Result.Success;
        }

        public void Activate() { if (!IsActive) IsActive = true; }
        public void Deactivate() { if (IsActive) IsActive = false; }
    }
}
