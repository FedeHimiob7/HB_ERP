using Identity.Domain.VO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Infrastructure.Persistence.Mappers
{
    public static class UserMapper
    {
        public static UserEntity ToModel(User user)
        {
            return new UserEntity
            {
                Id = user.Id.Value,

                UserName = user.Email.Value,
                NormalizedUserName = user.Email.Value.ToUpperInvariant(),

                Email = user.Email.Value,
                NormalizedEmail = user.Email.Value.ToUpperInvariant(),

                EmailConfirmed = false,

                PasswordHash = user.PasswordHash?.Value ?? throw new InvalidOperationException("PasswordHash cannot be null"),
                SecurityStamp = Guid.NewGuid().ToString(),
                ConcurrencyStamp = Guid.NewGuid().ToString(),

                PhoneNumber = null,
                PhoneNumberConfirmed = false,

                TwoFactorEnabled = false,
                LockoutEnabled = false,
                LockoutEnd = null,
                AccessFailedCount = 0,

                FirstName = user.FirstName,
                LastName = user.LastName,

                ViewAll = user.ViewAll,
                DefaultDashboard = string.Empty,
                IsActive = user.IsActive
            };
        }

        public static User ToDomain(UserEntity model)
        {
            return User.CreateExisting(
                id: model.Id,
                firstName: model.FirstName,
                lastName: model.LastName,
                email: Email.Create(model.Email).Value,
                passwordHash: PasswordHash.Create(model.PasswordHash),
                roles: model.UserRoles.Select(user => new RoleId(user.RoleId)),
                isActive: model.IsActive

            );
        }

        public static void MapToExistingEntity(User source, UserEntity target)
        {
            target.FirstName = source.FirstName;
            target.LastName = source.LastName;
            target.Email = source.Email.Value;
            target.NormalizedEmail = source.Email.Value.ToUpper();
            target.IsActive = source.IsActive;

            target.UserRoles = source.Roles
                .Select(r => new UserRoleEntity
                {
                    UserId = target.Id,
                    RoleId = r.Value
                })
                .ToList();
        }
       
    }
}
