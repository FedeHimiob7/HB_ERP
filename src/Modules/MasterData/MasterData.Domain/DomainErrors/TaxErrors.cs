using ErrorOr;

namespace MasterData.Domain.DomainErrors
{
    public static class TaxErrors
    {
        public static Error NameIsRequired => Error.Validation(
            code: "Tax.NameIsRequired",
            description: "El nombre del impuesto es obligatorio.");

        public static Error NotFound => Error.NotFound(
            code: "Tax.NotFound",
            description: "El impuesto solicitado no existe.");
    }
}
