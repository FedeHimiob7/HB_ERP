using ErrorOr;

namespace Inventory.Domain.DomainErrors
{
    public static class StorageTypeErrors
    {
        public static Error NameIsRequired => Error.Validation(
            code: "StorageType.NameIsRequired",
            description: "El nombre del tipo de almacenamiento es obligatorio.");

        public static Error NotFound => Error.NotFound(
            code: "StorageType.NotFound",
            description: "El tipo de almacenamiento solicitado no existe.");

        public static Error DuplicateName => Error.Conflict(
            code: "StorageType.DuplicateName",
            description: "Ya existe un tipo de almacenamiento con ese nombre.");
    }
}
