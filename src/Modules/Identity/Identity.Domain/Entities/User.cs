using Ardalis.GuardClauses;
using Identity.Domain.Entities;
using Identity.Domain.Events;
using Identity.Domain.VO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Domain
{
    public sealed class User : AggregateRoot<UserId>
    {
        private readonly List<RoleId> _roles = new();

        public string FirstName { get; private set; } = null!;
        public string LastName { get; private set; } = null!;
        public Email Email { get; private set; } = null!;
        public bool IsActive { get; private set; }
        public bool ViewAll { get; private set; }
        public PasswordHash? PasswordHash { get; private set; } 
        public IReadOnlyList<RoleId> Roles => _roles;
        private User() { }

        private User(UserId id, string firstName, string lastName, 
                     Email email, PasswordHash passwordHash)
        : base(id)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            PasswordHash = passwordHash;
            IsActive = true;
            ViewAll = false;
        }
        private User(UserId id, string firstName, string lastName, Email email, PasswordHash passwordHash,
                     List<RoleId>? roles = null, bool isActive = true)
        {
            Id = id;     
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            PasswordHash = passwordHash;
            _roles = roles ?? new();
            IsActive = isActive;
        }

        public static User Register(string firstName, string lastName, Email email, PasswordHash passwordHash)
        {
            var user = new User(UserId.New(), firstName, lastName, email, passwordHash);

            user.Raise(new UserRegisteredDomainEvent(
                Guid.NewGuid(),
                user.Id,
                user.Email
            ));

            return user;
        }

        public static User CreateExisting(Guid id, string firstName, string lastName, Email email,
                                          PasswordHash passwordHash, IEnumerable<RoleId>? roles, 
                                          bool isActive)
        {
            return new User(
                UserId.Create(id),
                firstName,
                lastName,
                email,
                passwordHash,
                roles?.ToList() ?? new List<RoleId>(),
                isActive
            );
        }

        public void ChangeFirstName(string firstName)
        {
            Guard.Against.NullOrWhiteSpace(firstName);
            FirstName = firstName;
        }

        public void ChangeLastName(string lastName)
        {
            Guard.Against.NullOrWhiteSpace(lastName);
            LastName = lastName;
        }

        public void ChangeEmail(Email email)
        {
            Guard.Against.Null(email);
            Email = email;
        }

        public void Activate()
        {
            if(IsActive)
                return;
            IsActive = true;
            Raise(new UserActivatedDomainEvent(
                Guid.NewGuid(),
                Id
            ));
        }
        public void Deactivate()
        {
            if (IsActive)
                return;
            IsActive = true;
            Raise(new UserDeactivatedDomainEvent(
                Guid.NewGuid(),
                Id
            ));
        }

        public void ActivateViewAll() => ViewAll = true;
        public void DeactivateViewAll() => ViewAll = false;

        public FullName Name => FullName.Create(FirstName, LastName);

        public void SetPasswordHash(PasswordHash hash)  
        {
            Guard.Against.Null(hash);
            PasswordHash = hash;
        }

        public void ChangePassword(PasswordHash newPasswordHash)
        {
            Guard.Against.Null(newPasswordHash);

            PasswordHash = newPasswordHash;

            Raise(new UserPasswordChangedDomainEvent(
                Guid.NewGuid(),
                Id
            ));
        }

        public void AssignRole(RoleId roleId)
        {
            Guard.Against.Null(roleId);

            if (_roles.Contains(roleId))
                return;

            _roles.Add(roleId);

            Raise(new UserRoleAssignedDomainEvent(
                Guid.NewGuid(),
                Id,
                roleId
            ));
        }

        public void RemoveRole(RoleId roleId)
        {
            Guard.Against.Null(roleId);

            if (!_roles.Contains(roleId))
                return;

            _roles.Remove(roleId);

            Raise(new UserRoleRemovedDomainEvent(
                Guid.NewGuid(),
                Id,
                roleId
            ));
        }

    }
}
