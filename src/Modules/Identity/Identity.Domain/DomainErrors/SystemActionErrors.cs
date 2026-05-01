using ErrorOr;
using Identity.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Domain.DomainErrors
{
    public static class SystemActionErrors
    {
        public static Error NotFound =>
            Error.Validation(
                code: ErrorCodes.SystemAction.ActionNotFound,
                description: FeaturedMessage.ActionNotFound);

        public static Error DuplicateName => 
            Error.Validation(
                code: ErrorCodes.SystemAction.DuplicateActionName,
                description: FeaturedMessage.NombreDeAccionYaRegistrado);

        public static Error InvalidActionName =>
            Error.Validation(
                code: ErrorCodes.SystemAction.InvalidActionName,
                description: FeaturedMessage.InvalidActionName);
    }
}
