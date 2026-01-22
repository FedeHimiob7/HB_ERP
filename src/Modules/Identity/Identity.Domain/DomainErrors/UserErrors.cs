using ErrorOr;
using Identity.Domain.Common;


namespace Identity.Domain.DomainErrors
{
    public static class UserErrors
    {
        public static Error InvalidEmail =>
            Error.Validation(
                code: ErrorCodes.User.InvalidEmail,
                description: FeaturedMessage.EmailInvalido);

        public static Error EmailAlreadyInUse =>
            Error.Validation(
                code: ErrorCodes.User.EmailAlreadyExists,
                description: FeaturedMessage.UsuarioEmailYaRegistrado);

        public static Error EmptyFirstName =>
            Error.Validation(
                code: ErrorCodes.User.UserFirstNameEmpty,
                description: FeaturedMessage.UserFirstNameVacio);

        public static Error EmptyLastName =>
            Error.Validation(
                code: ErrorCodes.User.UserLastNameEmpty,
                description: FeaturedMessage.UserLastNameVacio);

        public static Error NotFound =>
            Error.Validation(
                code: ErrorCodes.User.UserNotFound,
                description: FeaturedMessage.UserNotFound);
    }
}
