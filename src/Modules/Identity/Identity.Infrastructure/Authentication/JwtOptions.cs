using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Infrastructure.Authentication
{
    public sealed class JwtOptions
    {
        public const string SectionName = "Jwt";

        public string Issuer { get; init; } = default!;
        public string Audience { get; init; } = default!;
        public string Secret { get; init; } = default!;
        public int ExpirationMinutes { get; init; }
    }
}
