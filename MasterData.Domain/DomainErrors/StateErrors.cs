using ErrorOr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterData.Domain.DomainErrors
{
    public static class StateErrors
    {
        public static Error InvalidCode => Error.Validation(
            code: "State.InvalidCode",
            description: "El código del estado no puede estar vacío.");

        public static Error NameIsRequired => Error.Validation(
            code: "State.NameIsRequired",
            description: "El nombre del estado es obligatorio.");

        public static Error InvalidCountry => Error.Validation(
            code: "State.InvalidCountry",
            description: "El estado debe estar asociado a un país válido.");

        public static Error NotFound => Error.NotFound(
            code: "State.NotFound",
            description: "El estado o provincia solicitado no existe.");
    }
}
