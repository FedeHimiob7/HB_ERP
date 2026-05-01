

using System.ComponentModel;

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
        public const string UsuarioCreadoExitosamente = "Usuario creado exitosamente.";
        public const string UsuarioActualizadoExitosamente = "Usuario actualizado exitosamente.";
        public const string IdInvalido = "Id Invalido";

        // Users
        public const string ErrorAlEjecutarOperacion = "Error al ejecutar la operación.";
        public const string EmailInvalido = "El email no tiene un formato válido.";
        public const string UsuarioEmailYaRegistrado = "Este Email ya esta siendo utilizado.";
        public const string UserFirstNameVacio = "El nombre no puede estar vacío.";
        public const string UserLastNameVacio = "El Apellido no puede estar vacío.";
        public const string UserNotFound = "Usuario no encontrado.";
        public const string UserInvalidCredentials = "Credenciales inválidas.";
        public const string PasswordInvalido = "La contraseña debe contener al menos una letra, un número y un caracter especial.";

        // Roles
        public const string NombreDeRolYaRegistrado = "Este Nombre ya esta siendo utilizado.";
        public const string RoleNotFound = "Rol no encontrado.";

        // System Actions

        public const string NombreDeAccionYaRegistrado = "Este Nombre ya esta siendo utilizado.";
        public const string ActionNotFound = "Acción no encontrada.";
        public const string InvalidActionName = "El nombre de la acción es invalido.";

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


