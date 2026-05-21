using Identity.Application.Common.Interfaces;
using Identity.Domain.Repositories;
using Identity.Domain.VO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.Roles.Commands.AssignAction
{
    public sealed class AssignActionToRoleCommandHandler : IRequestHandler<AssignActionToRoleCommand, ErrorOr<Success>>
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AssignActionToRoleCommandHandler(
            IRoleRepository roleRepository,
            IUnitOfWork unitOfWork)
        {
            _roleRepository = roleRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Success>> Handle(AssignActionToRoleCommand request, CancellationToken cancellationToken)
        {
            // 1. Instanciar el strongly-typed ID
            var roleId = new RoleId(request.RoleId);

            // 2. Recuperar el Agregado de la base de datos (el repositorio ya incluye las acciones gracias al Include que configuramos)
            var role = await _roleRepository.GetByIdAsync(roleId, cancellationToken);

            if (role is null)
            {
                // Usamos Error.NotFound nativo de ErrorOr (o puedes usar un RoleErrors.NotFound si ya tienes una clase estática de errores para Role)
                return Error.NotFound("Role.NotFound", "El rol especificado no existe.");
            }

            // 3. Transformar el Guid entrante al Value Object requerido
            var actionId = new ActionsId(request.ActionId);

            // 4. Delegar la mutación del estado a la Entidad de Dominio (esto internamente validará duplicados y disparará el Domain Event)
            role.AssignAction(actionId);

            // 5. Actualizar y guardar los cambios en la base de datos
            await _roleRepository.UpdateAsync(role, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // 6. Retornar éxito
            return Result.Success;
        }
    }
}
