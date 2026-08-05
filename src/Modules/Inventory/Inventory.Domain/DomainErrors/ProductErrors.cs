using ErrorOr;

namespace Inventory.Domain.DomainErrors
{
    public static class ProductErrors
    {
        public static Error NameIsRequired => Error.Validation(
            code: "Product.NameIsRequired",
            description: "El nombre del producto es obligatorio.");

        public static Error CodeIsRequired => Error.Validation(
            code: "Product.CodeIsRequired",
            description: "El código del producto es obligatorio.");

        public static Error InvalidProductServiceLine => Error.Validation(
            code: "Product.InvalidProductServiceLine",
            description: "La línea de producto/servicio es obligatoria.");

        public static Error InvalidBranch => Error.Validation(
            code: "Product.InvalidBranch",
            description: "La sucursal indicada no es válida.");

        public static Error InvalidCostCurrency => Error.Validation(
            code: "Product.InvalidCostCurrency",
            description: "La moneda del costo no es válida.");

        public static Error InvalidPriceCurrency => Error.Validation(
            code: "Product.InvalidPriceCurrency",
            description: "La moneda del precio no es válida.");

        public static Error NegativeCost => Error.Validation(
            code: "Product.NegativeCost",
            description: "El costo no puede ser negativo.");

        public static Error NegativePrice => Error.Validation(
            code: "Product.NegativePrice",
            description: "El precio no puede ser negativo.");

        public static Error NotFound => Error.NotFound(
            code: "Product.NotFound",
            description: "El producto solicitado no existe.");

        public static Error DuplicateCode => Error.Conflict(
            code: "Product.DuplicateCode",
            description: "Ya existe un producto con ese código.");
    }
}
