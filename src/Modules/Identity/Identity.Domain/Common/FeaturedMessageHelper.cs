

namespace Identity.Domain.Common
{
    public static class FeaturedMessageHelper
    {
        public static string GetMessage(string message, params string[] messageParams)
        {
            if (messageParams == null || messageParams.Length == 0)
                return message;

            return string.Format(message, messageParams);
        }
    }

    public static class FeaturedMessage
    {
        // Users
        public const string ErrorAlEjecutarOperacion = "Error al ejecutar la operación.";
        public const string EmailInvalido = "El email no tiene un formato válido.";
        public const string UsuarioEmailYaRegistrado = "Este Email ya esta siendo utilizado.";
        public const string UserFirstNameVacio = "El nombre no puede estar vacío.";
        public const string UserLastNameVacio = "El Apellido no puede estar vacío.";
        public const string UserNotFound = "Usuario no encontrado.";

        // Roles
        public const string NombreDeRolYaRegistrado = "Este Nombre ya esta siendo utilizado.";
        public const string RoleNotFound = "Rol no encontrado.";
    }

    // Ejemplo de uso con parametros
    /*
        if (totalAmount > amountDue)
        {
            response.Message = FeaturedMessageHelper.GetMessage(
                FeaturedMessage.PagoExcedeTransaccionYCliente,
                trans.DocumentNumber,   **primer parámetro**
                client.Name             **segundo parámetro**
            );
        }
    */

}


