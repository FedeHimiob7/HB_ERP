using Identity.Application.Common.Interfaces;
using Identity.Application.Users.Commands.RegisterUser;
using Identity.Domain;
using Identity.Domain.Common;
using Identity.Domain.DomainErrors;
using Identity.Domain.Entities;
using Identity.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.Roles.Commands.RegisterRole
{
    public sealed class RegisterRoleCommandHandler 
        : IRequestHandler<RegisterRoleCommand, ErrorOr<Guid>>
    {
        private readonly IRoleRepository _RoleRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRoleNameUniquenessChecker _roleNameUniquenessChecker;

        public RegisterRoleCommandHandler(
            IRoleRepository RoleRepository,
            IUnitOfWork unitOfWork,
            IRoleNameUniquenessChecker roleNameUniquenessChecker)
        {
            _RoleRepository = RoleRepository
                ?? throw new ArgumentNullException(nameof(RoleRepository));
            _unitOfWork = unitOfWork
                ?? throw new ArgumentNullException(nameof(unitOfWork));
            _roleNameUniquenessChecker = roleNameUniquenessChecker
                ?? throw new ArgumentNullException(nameof(roleNameUniquenessChecker));
        }

        public async Task<ErrorOr<Guid>> Handle(
            RegisterRoleCommand command,
            CancellationToken cancellationToken)
        {
            if (!await _roleNameUniquenessChecker
                .IsRoleNameUniqueAsync(command.Name, cancellationToken))
            {
                return RoleErrors.NameAlreadyInUse;
            }

            var role = Role.Create(command.Name);
            await _RoleRepository.AddAsync(role);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return role.Id.Value;
        }
    }
}
