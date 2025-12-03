using Ardalis.GuardClauses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Domain.VO
{
    public sealed record class PasswordHash
    {
        public string Value { get; }

        private PasswordHash(string value)
        {
            Guard.Against.NullOrWhiteSpace(value);
            Value = value;
        }

        public static PasswordHash Create(string hash)
            => new(hash.Trim());

        public override string ToString() => Value;
    }
}
