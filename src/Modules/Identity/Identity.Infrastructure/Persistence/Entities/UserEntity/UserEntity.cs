using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Infrastructure.Persistence
{
    public class UserEntity
    {
        public Guid Id { get; set; }
        public string UserName { get; set; } = default!;
        public string NormalizedUserName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string NormalizedEmail { get; set; } = default!;
        public bool EmailConfirmed { get; set; }
        public string PasswordHash { get; set; } = default!;
        public string SecurityStamp { get; set; } = default!;
        public string ConcurrencyStamp { get; set; } = default!;
        public string? PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public DateTime? LockoutEnd { get; set; }
        public bool LockoutEnabled { get; set; }
        public int AccessFailedCount { get; set; }

        // Campos del ERP viejo
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public bool ViewAll { get; set; }
        public string DefaultDashboard { get; set; }
        public bool IsActive { get; set; }


        public ICollection<UserRoleEntity> UserRoles { get; set; } = new List<UserRoleEntity>();
    }
}
