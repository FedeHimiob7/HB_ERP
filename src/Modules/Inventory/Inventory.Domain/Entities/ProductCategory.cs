using ErrorOr;
using HB_ERP.SharedKernel.Domain.Primitives;
using Inventory.Domain.DomainErrors;
using Inventory.Domain.Events;
using Inventory.Domain.VO;
using MasterData.Domain.VO;

namespace Inventory.Domain.Entities
{
    public sealed class ProductCategory : AggregateRoot<ProductCategoryId>
    {
        private ProductCategory() { }

        private ProductCategory(
            ProductCategoryId id,
            ProductServiceLineId productServiceLineId,
            ProductTypeId? productTypeId,
            string name,
            string? description,
            bool isActive)
            : base(id)
        {
            ProductServiceLineId = productServiceLineId;
            ProductTypeId = productTypeId;
            Name = name;
            Description = description;
            IsActive = isActive;
        }

        public ProductServiceLineId ProductServiceLineId { get; private set; }
        public ProductTypeId? ProductTypeId { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public string? Description { get; private set; }
        public bool IsActive { get; private set; }

        public static ErrorOr<ProductCategory> Create(
            ProductServiceLineId productServiceLineId,
            ProductTypeId? productTypeId,
            string name,
            string? description)
        {
            if (productServiceLineId.Value == Guid.Empty)
                return ProductCategoryErrors.InvalidProductServiceLine;

            if (string.IsNullOrWhiteSpace(name))
                return ProductCategoryErrors.NameIsRequired;

            var category = new ProductCategory(
                ProductCategoryId.New(),
                productServiceLineId,
                productTypeId,
                name.Trim(),
                description?.Trim(),
                isActive: true);

            category.Raise(new ProductCategoryCreatedDomainEvent(
                category.Id,
                category.ProductServiceLineId,
                category.Name));

            return category;
        }

        public static ProductCategory CreateExisting(
            Guid id,
            Guid productServiceLineId,
            Guid? productTypeId,
            string name,
            string? description,
            bool isActive)
        {
            ProductTypeId? typeId = productTypeId is Guid pid ? new ProductTypeId(pid) : null;
            return new ProductCategory(
                ProductCategoryId.Create(id),
                ProductServiceLineId.Create(productServiceLineId),
                typeId,
                name, description, isActive);
        }

        public ErrorOr<Success> UpdateDetails(
            ProductServiceLineId productServiceLineId,
            ProductTypeId? productTypeId,
            string name,
            string? description)
        {
            if (productServiceLineId.Value == Guid.Empty)
                return ProductCategoryErrors.InvalidProductServiceLine;

            if (string.IsNullOrWhiteSpace(name))
                return ProductCategoryErrors.NameIsRequired;

            ProductServiceLineId = productServiceLineId;
            ProductTypeId = productTypeId;
            Name = name.Trim();
            Description = description?.Trim();
            return Result.Success;
        }

        public void Activate() { if (!IsActive) IsActive = true; }
        public void Deactivate() { if (IsActive) IsActive = false; }
    }
}
