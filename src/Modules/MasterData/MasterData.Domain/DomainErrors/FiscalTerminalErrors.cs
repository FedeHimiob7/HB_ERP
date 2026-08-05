using ErrorOr;

namespace MasterData.Domain.DomainErrors
{
    public static class FiscalTerminalErrors
    {
        public static Error NameIsRequired => Error.Validation(
            code: "FiscalTerminal.NameIsRequired",
            description: "El nombre del punto de emisión es obligatorio.");

        public static Error InvalidBranch => Error.Validation(
            code: "FiscalTerminal.InvalidBranch",
            description: "El punto de emisión debe estar asociado a una sucursal válida.");

        public static Error NotFound => Error.NotFound(
            code: "FiscalTerminal.NotFound",
            description: "El punto de emisión solicitado no existe.");
    }
}
