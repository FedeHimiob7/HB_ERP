using ErrorOr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterData.Domain.DomainErrors
{
    public static class ProductServiceLineErrors
    {
        public static Error NameIsRequired => Error.Validation(
            code: "ProductServiceLine.NameIsRequired",
            description: "El nombre de la línea de servicio no puede estar vacío.");

        public static Error NotFound => Error.NotFound(
            code: "ProductServiceLine.NotFound",
            description: "La línea de servicio de producto solicitada no existe.");
    }
}
