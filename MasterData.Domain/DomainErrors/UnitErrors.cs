using ErrorOr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterData.Domain.DomainErrors
{
    public static class UnitErrors
    {
        public static Error NameIsRequired => Error.Validation(
            code: "Unit.NameIsRequired",
            description: "El nombre de la unidad es obligatorio.");

        public static Error DescriptionIsRequired => Error.Validation(
            code: "Unit.DescriptionIsRequired",
            description: "La descripción de la unidad es obligatoria.");

        public static Error NotFound => Error.NotFound(
            code: "Unit.NotFound",
            description: "La unidad de medida solicitada no existe.");
    }
}
