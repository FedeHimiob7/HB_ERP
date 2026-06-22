using HB_ERP.SharedKernel.Domain.Primitives;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace HB_ERP.SharedKernel.Application.Security
{
    public sealed class CurrentUserProvider : ICurrentUserProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserProvider(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        // Intenta leer el NameIdentifier clásico o el 'sub' estándar de OpenID/JWT
        public string? UserId => _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                                 ?? _httpContextAccessor.HttpContext?.User?.FindFirst("sub")?.Value;

        public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

        public IReadOnlyList<Guid> PslIds =>
            _httpContextAccessor.HttpContext?.User?.FindAll("psl_ids")
                .Select(c => Guid.TryParse(c.Value, out var g) ? g : (Guid?)null)
                .Where(g => g.HasValue)
                .Select(g => g!.Value)
                .ToList() ?? new List<Guid>();
    }
}
