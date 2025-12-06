using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Domain.Common
{
    public static class ErrorCodes
    {
        public static class User
        {
            public const string InvalidEmail = "User.InvalidEmail";
            public const string EmailAlreadyExists = "User.EmailAlreadyExists";
            public const string PasswordWeak = "User.PasswordWeak";
            public const string UserFirstNameEmpty = "User.EmptyFirstName";
            public const string UserLastNameEmpty = "User.EmptyLastName";
        }
    }
}
