using ErrorOr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Domain.DomainErrors
{
    public static class UserErrors
    {
        public static Error InvalidEmail =>
            Error.Validation(
                code: "User.InvalidEmail",
                description: "El email no tiene un formato válido.");

        public static Error EmptyFirstName =>
            Error.Validation(
                code: "User.EmptyFirstName",
                description: "El nombre no puede estar vacío.");

        public static Error EmptyLastName =>
            Error.Validation(
                code: "User.EmptyLastName",
                description: "El apellido no puede estar vacío.");

        public static Error AlreadyInactive =>
            Error.Validation(
                code: "User.AlreadyInactive",
                description: "El usuario ya está desactivado.");
    }
}
