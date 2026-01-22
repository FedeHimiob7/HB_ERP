using ErrorOr;
using Identity.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Domain.DomainErrors
{
    public static class RoleErrors
    {
        public static Error NameAlreadyInUse =>
            Error.Validation(
                code: ErrorCodes.Role.DuplicateRoleName,
                description: FeaturedMessage.NombreDeRolYaRegistrado);

        public static Error RoleNotFound =>
            Error.NotFound(
                code: ErrorCodes.Role.RoleNotFound,
                description: FeaturedMessage.RoleNotFound);
    }
}
