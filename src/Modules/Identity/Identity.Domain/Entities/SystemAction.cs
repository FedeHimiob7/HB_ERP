using Ardalis.GuardClauses;
using Identity.Domain.VO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Domain.Entities
{
    public sealed class SystemAction : AggregateRoot<ActionsId>
    {
        public string Name { get; private set; } = null!;
        public string Description { get; private set; } = null!;
        public bool IsActive { get; private set; } 

        private SystemAction() { }

        private SystemAction(ActionsId id, string name, string description, bool isActive) : base(id)
        {
            UpdateDetails(name, description);
            IsActive = isActive;
        }
        
        public static SystemAction Create(string name, string description)
        {
            return new SystemAction(ActionsId.New(), name, description, true);
        }

        public static SystemAction CreateExisting(ActionsId id, string name, string description, bool isActive)
        {
            return new SystemAction(id, name, description, isActive);
        }

        public void UpdateDetails(string name, string description)
        {
            Guard.Against.NullOrWhiteSpace(name, nameof(name));
            Guard.Against.NullOrWhiteSpace(description, nameof(description));

            Name = name;
            Description = description;
        }

        public void Activate() => IsActive = true;

        public void Deactivate() => IsActive = false;
    }
}
