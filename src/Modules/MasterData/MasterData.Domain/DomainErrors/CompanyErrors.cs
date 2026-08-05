using ErrorOr;

namespace MasterData.Domain.DomainErrors
{
    public static class CompanyErrors
    {
        public static Error RifIsRequired => Error.Validation(
            code: "Company.RifIsRequired",
            description: "El RIF de la empresa es obligatorio.");

        public static Error InvalidRifFormat => Error.Validation(
            code: "Company.InvalidRifFormat",
            description: "El RIF no tiene un formato válido (ej. J-401027631-4).");

        public static Error LegalNameIsRequired => Error.Validation(
            code: "Company.LegalNameIsRequired",
            description: "La razón social de la empresa es obligatoria.");

        public static Error RegisteredAddressIsRequired => Error.Validation(
            code: "Company.RegisteredAddressIsRequired",
            description: "El domicilio fiscal de la empresa es obligatorio.");

        public static Error AlreadyExists => Error.Conflict(
            code: "Company.AlreadyExists",
            description: "Ya existe una empresa configurada en esta instalación.");

        public static Error NotConfigured => Error.NotFound(
            code: "Company.NotConfigured",
            description: "Todavía no se ha configurado la empresa de esta instalación.");
    }
}
