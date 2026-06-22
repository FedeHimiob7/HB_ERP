using ErrorOr;

namespace HB_ERP.SharedKernel.Domain
{
    public static class CommonErrors
    {
        public static Error PslAccessDenied => Error.Forbidden(
            code: "PSL.AccessDenied",
            description: "No tiene acceso a la línea de servicio indicada.");
    }
}
