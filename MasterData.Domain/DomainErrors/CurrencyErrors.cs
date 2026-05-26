using ErrorOr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterData.Domain.DomainErrors
{
    public static class CurrencyErrors
    {
        public static Error InvalidCode => Error.Validation(
            code: "Currency.InvalidCode",
            description: "El código ISO de la moneda debe tener exactamente 3 caracteres.");

        public static Error NameIsRequired => Error.Validation(
            code: "Currency.NameIsRequired",
            description: "El nombre de la moneda no puede estar vacío.");
    }
}
