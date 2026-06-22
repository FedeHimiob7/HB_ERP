using ErrorOr;

namespace Inventory.Domain.DomainErrors
{
    public static class WarehouseErrors
    {
        public static Error NameIsRequired => Error.Validation(
            code: "Warehouse.NameIsRequired",
            description: "El nombre del almacén es obligatorio.");

        public static Error InvalidProductServiceLine => Error.Validation(
            code: "Warehouse.InvalidProductServiceLine",
            description: "El almacén debe estar asociado a una línea de servicio válida.");

        public static Error NotFound => Error.NotFound(
            code: "Warehouse.NotFound",
            description: "El almacén solicitado no existe.");

        public static Error DuplicateNameInPsl => Error.Conflict(
            code: "Warehouse.DuplicateNameInPsl",
            description: "Ya existe un almacén con ese nombre en esta línea de servicio.");
    }
}
