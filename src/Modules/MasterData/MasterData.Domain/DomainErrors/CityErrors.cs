using ErrorOr;

namespace MasterData.Domain.DomainErrors
{
    public static class CityErrors
    {
        public static Error NameIsRequired => Error.Validation(
            code: "City.NameIsRequired",
            description: "El nombre de la ciudad es obligatorio.");

        public static Error InvalidState => Error.Validation(
            code: "City.InvalidState",
            description: "La ciudad debe estar asociada a un estado/provincia válido.");

        public static Error NotFound => Error.NotFound(
            code: "City.NotFound",
            description: "La ciudad solicitada no existe.");

        public static Error DuplicateName => Error.Conflict(
            code: "City.DuplicateName",
            description: "Ya existe una ciudad con ese nombre en el estado indicado.");
    }
}
