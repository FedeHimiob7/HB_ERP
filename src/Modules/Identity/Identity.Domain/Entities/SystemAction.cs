using Ardalis.GuardClauses;
using Identity.Domain.Events;
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
        public ActionName Name { get; private set; } = null!;
        public string Description { get; private set; } = null!;
        public bool IsActive { get; private set; }

        private SystemAction() { }

        private SystemAction(ActionsId id, ActionName name, string description, bool isActive) : base(id)
        {
            Name = name;
            Description = description;
            IsActive = isActive;
        }

        public static SystemAction Create(string name, string description)
        {
            var actionName = ActionName.Create(name);
            var actionId = new ActionsId(Guid.NewGuid());

            var systemAction = new SystemAction(actionId, actionName, description, true);

           
            systemAction.Raise(new SystemActionCreatedDomainEvent(
                Guid.NewGuid(),
                systemAction.Id.Value,
                actionName.Value));

            return systemAction;
        }

        public static SystemAction CreateExisting(ActionsId id, ActionName name, string description, bool isActive)
        {
            return new SystemAction(id, name, description, isActive);
        }

        public void UpdateDetails(ActionName name, string description)
        {
            Guard.Against.NullOrWhiteSpace(description, nameof(description));

            Name = name;
            Description = description;

            // Aquí también podrías disparar un SystemActionUpdatedDomainEvent
        }

        public void Activate() => IsActive = true;

        public void Deactivate() => IsActive = false;
    }
}
