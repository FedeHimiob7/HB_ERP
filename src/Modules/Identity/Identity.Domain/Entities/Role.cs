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
    public sealed class Role : AggregateRoot<RoleId>
    {
        public string Name { get; private set; } = null!;

        private readonly List<ActionsId> _actionIds = new();

        public IReadOnlyCollection<ActionsId> ActionIds => _actionIds.AsReadOnly();

        public bool IsActive { get; private set; }

        private Role() { }

        private Role(RoleId id, string name) : base(id)
        {
            ChangeName(name);
            IsActive = true;
        }

        public static Role Create(string name)
        {
            return new Role(RoleId.New(), name);
        }
        public static Role CreateExisting(RoleId id, string name)
        {
            return new Role(id, name);
        }

        public void ChangeName(string name)
        {
            Guard.Against.NullOrWhiteSpace(name);
            Name = name;
        }

        public void AssignAction(ActionsId actionId)
        {
            if (!_actionIds.Contains(actionId))
            {
                _actionIds.Add(actionId);

                Raise(new ActionAssignedToRoleDomainEvent(
                    Guid.NewGuid(),
                    this.Id,
                    actionId));
            }
        }

        public void RevokeAction(ActionsId actionId)
        {
            if (_actionIds.Contains(actionId))
            {
                _actionIds.Remove(actionId);

                Raise(new ActionRevokedFromRoleDomainEvent(
                    Guid.NewGuid(),
                    this.Id,
                    actionId));
            }
        }

        public void Activate() => IsActive = true;

        public void Deactivate() => IsActive = false;
    }
}
