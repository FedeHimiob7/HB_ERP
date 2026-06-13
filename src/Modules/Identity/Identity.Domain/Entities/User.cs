using Ardalis.GuardClauses;
using HB_ERP.SharedKernel.Domain.Primitives;
using Identity.Domain.Events;
using Identity.Domain.VO;

namespace Identity.Domain
{
    public sealed class User : AggregateRoot<UserId>
    {
        private readonly List<RoleId> _roles = new();
        private readonly List<PslId> _psls = new();

        public string FirstName { get; private set; } = null!;
        public string LastName { get; private set; } = null!;
        public Email Email { get; private set; } = null!;
        public bool IsActive { get; private set; }
        public bool ViewAll { get; private set; }
        public PasswordHash? PasswordHash { get; private set; } 
        public IReadOnlyList<RoleId> Roles => _roles;
        public IReadOnlyList<PslId> Psls => _psls;
        private User() { }

        private User(UserId id, string firstName, string lastName, Email email, PasswordHash passwordHash,
                     List<RoleId>? roles = null, List<PslId>? psls = null, bool isActive = true)
            : base(id)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            PasswordHash = passwordHash;
            _roles = roles ?? new();
            _psls = psls ?? new();
            IsActive = isActive;
            ViewAll = false;
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
                                          IEnumerable<PslId>? psls, bool isActive)
        {
            return new User(
                UserId.Create(id),
                firstName,
                lastName,
                email,
                passwordHash,
                roles?.ToList() ?? new List<RoleId>(),
                psls?.ToList() ?? new List<PslId>(),
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
            IsActive = false;
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

        public void SyncRoles(IEnumerable<RoleId> newRoles)
        {
            Guard.Against.Null(newRoles);

            var toAdd = newRoles.Except(_roles).ToList();
            var toRemove = _roles.Except(newRoles).ToList();

            foreach (var role in toAdd)
                AssignRole(role);

            foreach (var role in toRemove)
                RemoveRole(role);
        }

        public void AssignPsl(PslId pslId)
        {
            Guard.Against.Null(pslId);

            if (_psls.Contains(pslId))
                return;

            _psls.Add(pslId);

            Raise(new UserPslAssignedDomainEvent(
                Guid.NewGuid(),
                Id,
                pslId
            ));
        }

        public void RemovePsl(PslId pslId)
        {
            Guard.Against.Null(pslId);

            if (!_psls.Contains(pslId))
                return;

            _psls.Remove(pslId);

            Raise(new UserPslRemovedDomainEvent(
                Guid.NewGuid(),
                Id,
                pslId
            ));
        }

        public void SyncPsls(IEnumerable<PslId> newPsls)
        {
            Guard.Against.Null(newPsls);

            var toAdd = newPsls.Except(_psls).ToList();
            var toRemove = _psls.Except(newPsls).ToList();

            foreach (var psl in toAdd)
                AssignPsl(psl);

            foreach (var psl in toRemove)
                RemovePsl(psl);
        }
    }
}
