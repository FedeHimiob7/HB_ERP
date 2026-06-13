using Identity.Domain.Events;
using Identity.Domain.Repositories;
using Identity.Domain.VO;
using MediatR;

namespace Identity.Application.SystemActions.DomainEventHandlers
{
    public sealed class SystemActionDeactivatedDomainEventHandler
        : INotificationHandler<SystemActionDeletedDomainEvent>
    {
        private readonly IRoleRepository _roleRepository;

        public SystemActionDeactivatedDomainEventHandler(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public async Task Handle(
            SystemActionDeletedDomainEvent notification,
            CancellationToken cancellationToken)
        {
            var roles = await _roleRepository.GetRolesByActionIdAsync(
                notification.ActionId,
                cancellationToken);

            if (!roles.Any()) return;

            var actionId = new ActionsId(notification.ActionId);
            foreach (var role in roles)
            {
                role.RevokeAction(actionId);
                await _roleRepository.UpdateAsync(role, cancellationToken);
            }
        }
    }
}
