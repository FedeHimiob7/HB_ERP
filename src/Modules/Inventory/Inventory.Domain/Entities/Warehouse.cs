using ErrorOr;
using HB_ERP.SharedKernel.Domain.Primitives;
using Inventory.Domain.DomainErrors;
using Inventory.Domain.Events;
using Inventory.Domain.VO;
using MasterData.Domain.VO;

namespace Inventory.Domain.Entities
{
    public sealed class Warehouse : AggregateRoot<WarehouseId>
    {
        private Warehouse() { }

        private Warehouse(
            WarehouseId id,
            ProductServiceLineId productServiceLineId,
            string name,
            string? description,
            string? latitude,
            string? longitude,
            bool isActive)
            : base(id)
        {
            ProductServiceLineId = productServiceLineId;
            Name = name;
            Description = description;
            Latitude = latitude;
            Longitude = longitude;
            IsActive = isActive;
        }

        public ProductServiceLineId ProductServiceLineId { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public string? Description { get; private set; }
        public string? Latitude { get; private set; }
        public string? Longitude { get; private set; }
        public bool IsActive { get; private set; }

        public static ErrorOr<Warehouse> Create(
            ProductServiceLineId productServiceLineId,
            string name,
            string? description,
            string? latitude,
            string? longitude)
        {
            if (productServiceLineId.Value == Guid.Empty)
                return WarehouseErrors.InvalidProductServiceLine;

            if (string.IsNullOrWhiteSpace(name))
                return WarehouseErrors.NameIsRequired;

            var warehouse = new Warehouse(
                WarehouseId.New(),
                productServiceLineId,
                name.Trim(),
                description?.Trim(),
                latitude?.Trim(),
                longitude?.Trim(),
                isActive: true);

            warehouse.Raise(new WarehouseCreatedDomainEvent(
                warehouse.Id,
                warehouse.ProductServiceLineId,
                warehouse.Name));

            return warehouse;
        }

        public static Warehouse CreateExisting(
            Guid id,
            Guid productServiceLineId,
            string name,
            string? description,
            string? latitude,
            string? longitude,
            bool isActive)
            => new Warehouse(
                WarehouseId.Create(id),
                ProductServiceLineId.Create(productServiceLineId),
                name, description, latitude, longitude, isActive);

        public ErrorOr<Success> UpdateDetails(
            ProductServiceLineId productServiceLineId,
            string name,
            string? description,
            string? latitude,
            string? longitude)
        {
            if (productServiceLineId.Value == Guid.Empty)
                return WarehouseErrors.InvalidProductServiceLine;

            if (string.IsNullOrWhiteSpace(name))
                return WarehouseErrors.NameIsRequired;

            ProductServiceLineId = productServiceLineId;
            Name = name.Trim();
            Description = description?.Trim();
            Latitude = latitude?.Trim();
            Longitude = longitude?.Trim();
            return Result.Success;
        }

        public void Activate() { if (!IsActive) IsActive = true; }
        public void Deactivate() { if (IsActive) IsActive = false; }
    }
}
