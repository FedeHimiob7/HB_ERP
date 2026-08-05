using ErrorOr;

namespace MasterData.Domain.DomainErrors
{
    public static class BranchErrors
    {
        public static Error NameIsRequired => Error.Validation(
            code: "Branch.NameIsRequired",
            description: "El nombre de la sucursal es obligatorio.");

        public static Error AddressIsRequired => Error.Validation(
            code: "Branch.AddressIsRequired",
            description: "La dirección de la sucursal es obligatoria.");

        public static Error NotFound => Error.NotFound(
            code: "Branch.NotFound",
            description: "La sucursal solicitada no existe.");
    }
}
