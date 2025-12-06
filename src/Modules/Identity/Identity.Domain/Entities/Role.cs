using Ardalis.GuardClauses;
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

        public void Activate() => IsActive = true;

        public void Deactivate() => IsActive = false;
    }
}
