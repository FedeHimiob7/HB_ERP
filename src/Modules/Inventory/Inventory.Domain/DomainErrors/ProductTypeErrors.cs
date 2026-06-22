using ErrorOr;

namespace Inventory.Domain.DomainErrors
{
    public static class ProductTypeErrors
    {
        public static Error NameIsRequired => Error.Validation(
            code: "ProductType.NameIsRequired",
            description: "El nombre del tipo de producto es obligatorio.");

        public static Error NotFound => Error.NotFound(
            code: "ProductType.NotFound",
            description: "El tipo de producto solicitado no existe.");

        public static Error DuplicateName => Error.Conflict(
            code: "ProductType.DuplicateName",
            description: "Ya existe un tipo de producto con ese nombre.");
    }
}
