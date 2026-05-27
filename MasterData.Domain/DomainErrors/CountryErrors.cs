using ErrorOr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterData.Domain.DomainErrors
{
    public static class CountryErrors
    {     
        public static Error NameIsRequired => Error.Validation(
            code: "Country.NameIsRequired",
            description: "El nombre del país no puede estar vacío.");

        public static Error NotFound => Error.NotFound(
            code: "Country.NotFound",
            description: "El país solicitado no existe.");
    }
}
