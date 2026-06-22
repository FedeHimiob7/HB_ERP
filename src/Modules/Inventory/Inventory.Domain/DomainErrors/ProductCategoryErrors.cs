using ErrorOr;

namespace Inventory.Domain.DomainErrors
{
    public static class ProductCategoryErrors
    {
        public static Error NameIsRequired => Error.Validation(
            code: "ProductCategory.NameIsRequired",
            description: "El nombre de la categoría es obligatorio.");

        public static Error InvalidProductServiceLine => Error.Validation(
            code: "ProductCategory.InvalidProductServiceLine",
            description: "La categoría debe estar asociada a una línea de servicio válida.");

        public static Error NotFound => Error.NotFound(
            code: "ProductCategory.NotFound",
            description: "La categoría solicitada no existe.");

        public static Error DuplicateNameInPsl => Error.Conflict(
            code: "ProductCategory.DuplicateNameInPsl",
            description: "Ya existe una categoría con ese nombre en esta línea de servicio.");

        public static Error InvalidProductType => Error.Validation(
            code: "ProductCategory.InvalidProductType",
            description: "El tipo de producto especificado no existe.");
    }
}
