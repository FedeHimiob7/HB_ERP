using ErrorOr;
using Identity.Domain.DomainErrors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Identity.Domain.VO
{
    public sealed record Email
    {
        private static readonly Regex EmailRegex =
            new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

        public string Value { get; }

        private Email(string value)
        {
            Value = value;
        }

        public static ErrorOr<Email> Create(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return UserErrors.InvalidEmail;

            email = email.Trim().ToLowerInvariant();

            if (!EmailRegex.IsMatch(email))
                return UserErrors.InvalidEmail;

            return new Email(email);
        }

        public override string ToString() => Value;
    }
}
