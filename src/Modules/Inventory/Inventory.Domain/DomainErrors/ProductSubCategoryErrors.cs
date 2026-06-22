using ErrorOr;

namespace Inventory.Domain.DomainErrors
{
    public static class ProductSubCategoryErrors
    {
        public static Error NameIsRequired => Error.Validation(
            code: "ProductSubCategory.NameIsRequired",
            description: "El nombre de la subcategoría es obligatorio.");

        public static Error InvalidCategory => Error.Validation(
            code: "ProductSubCategory.InvalidCategory",
            description: "La subcategoría debe estar asociada a una categoría válida.");

        public static Error NotFound => Error.NotFound(
            code: "ProductSubCategory.NotFound",
            description: "La subcategoría solicitada no existe.");

        public static Error DuplicateNameInCategory => Error.Conflict(
            code: "ProductSubCategory.DuplicateNameInCategory",
            description: "Ya existe una subcategoría con ese nombre en esta categoría.");
    }
}
