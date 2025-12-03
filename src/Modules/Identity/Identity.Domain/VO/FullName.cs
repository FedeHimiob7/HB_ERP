using Ardalis.GuardClauses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Domain.VO
{
    public sealed record class FullName
    {
        public string FirstName { get; }
        public string LastName { get; }

        private FullName(string firstName, string lastName)
        {
            Guard.Against.NullOrWhiteSpace(firstName);
            Guard.Against.NullOrWhiteSpace(lastName);

            FirstName = firstName;
            LastName = lastName;
        }

        public static FullName Create(string firstName, string lastName)
            => new(firstName.Trim(), lastName.Trim());

        public override string ToString() => $"{FirstName} {LastName}";
    }
}
