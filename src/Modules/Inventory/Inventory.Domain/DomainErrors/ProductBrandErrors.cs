using ErrorOr;

namespace Inventory.Domain.DomainErrors
{
    public static class ProductBrandErrors
    {
        public static Error NameIsRequired => Error.Validation(
            code: "ProductBrand.NameIsRequired",
            description: "El nombre de la marca es obligatorio.");

        public static Error NotFound => Error.NotFound(
            code: "ProductBrand.NotFound",
            description: "La marca solicitada no existe.");

        public static Error DuplicateName => Error.Conflict(
            code: "ProductBrand.DuplicateName",
            description: "Ya existe una marca con ese nombre.");
    }
}
