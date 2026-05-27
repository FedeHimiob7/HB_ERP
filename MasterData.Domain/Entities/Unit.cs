using ErrorOr;
using HB_ERP.SharedKernel.Domain.Primitives;
using MasterData.Domain.DomainErrors;
using MasterData.Domain.Events;
using MasterData.Domain.VO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterData.Domain.Entities
{
    public sealed class Unit : AggregateRoot<UnitId>
    {
        private Unit() { }

        private Unit(UnitId id, string name, string description, bool isActive) : base(id)
        {
            Name = name;
            Description = description;
            IsActive = isActive;
        }

        public string Name { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;
        public bool IsActive { get; private set; }

        public static ErrorOr<Unit> Create(string name, string description)
        {
            if (string.IsNullOrWhiteSpace(name)) return UnitErrors.NameIsRequired;
            if (string.IsNullOrWhiteSpace(description)) return UnitErrors.DescriptionIsRequired;

            var unit = new Unit(UnitId.New(), name.Trim(), description.Trim(), isActive: true);

            unit.Raise(new UnitCreatedDomainEvent(unit.Id, unit.Name, unit.Description));

            return unit;
        }

        public ErrorOr<Success> UpdateDetails(string name, string description)
        {
            if (string.IsNullOrWhiteSpace(name)) return UnitErrors.NameIsRequired;
            if (string.IsNullOrWhiteSpace(description)) return UnitErrors.DescriptionIsRequired;

            Name = name.Trim();
            Description = description.Trim();

            return Result.Success;
        }

        public void Deactivate()
        {
            if (!IsActive) return;
            IsActive = false;
        }

        public void Activate()
        {
            if (IsActive) return;
            IsActive = true;
        }
    }
}
