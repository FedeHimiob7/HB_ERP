using ErrorOr;
using HB_ERP.SharedKernel.Domain.Primitives;
using Inventory.Domain.DomainErrors;
using Inventory.Domain.Events;
using Inventory.Domain.VO;

namespace Inventory.Domain.Entities
{
    public sealed class StorageType : AggregateRoot<StorageTypeId>
    {
        private StorageType() { }

        private StorageType(StorageTypeId id, string name, string? description, bool isActive)
            : base(id)
        {
            Name = name;
            Description = description;
            IsActive = isActive;
        }

        public string Name { get; private set; } = string.Empty;
        public string? Description { get; private set; }
        public bool IsActive { get; private set; }

        public static ErrorOr<StorageType> Create(string name, string? description)
        {
            if (string.IsNullOrWhiteSpace(name))
                return StorageTypeErrors.NameIsRequired;

            var storageType = new StorageType(StorageTypeId.New(), name.Trim(), description?.Trim(), isActive: true);
            storageType.Raise(new StorageTypeCreatedDomainEvent(storageType.Id, storageType.Name));
            return storageType;
        }

        public static StorageType CreateExisting(Guid id, string name, string? description, bool isActive)
            => new StorageType(StorageTypeId.Create(id), name, description, isActive);

        public ErrorOr<Success> UpdateDetails(string name, string? description)
        {
            if (string.IsNullOrWhiteSpace(name))
                return StorageTypeErrors.NameIsRequired;

            Name = name.Trim();
            Description = description?.Trim();
            return Result.Success;
        }

        public void Activate() { if (!IsActive) IsActive = true; }
        public void Deactivate() { if (IsActive) IsActive = false; }
    }
}
