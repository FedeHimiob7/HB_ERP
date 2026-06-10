using Identity.Application.Common.Interfaces;
using Identity.Infrastructure.Persistence;
using Identity.Infrastructure.Persistence.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Infrastructure.Authentication
{
    internal sealed class JwtTokenService : IJwtTokenService
    {
        private readonly JwtOptions _options;
        private readonly IdentityDbContext _context;

        public JwtTokenService(IOptions<JwtOptions> options, IdentityDbContext context)
        {
            _options = options.Value;
            _context = context;
        }

        public async Task<string> GenerateTokenAsync(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);


            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Id.Value.ToString()),
                new(JwtRegisteredClaimNames.Email, user.Email.Value),
                new(JwtRegisteredClaimNames.UniqueName, user.FirstName)
            };


            var userRoleIds = user.Roles.Select(r => r.Value).ToList();

            var roleNames = await _context.Roles
                .Where(r => userRoleIds.Contains(r.Id))
                .Select(r => r.Name)
                .ToListAsync();

            foreach (var roleName in roleNames)
            {
                claims.Add(new Claim("roles", roleName));
            }

            var permissions = await _context.Set<RoleActionEntity>()
                .Where(ra => userRoleIds.Contains(ra.RoleId))
                .Select(ra => ra.SystemAction)
                .Where(sa => sa != null && sa.IsActive)
                .Select(sa => sa.Name)
                .Distinct()
                .ToListAsync();

            foreach (var permissionName in permissions)
            {
                claims.Add(new Claim("permissions", permissionName));
            }

            var token = new JwtSecurityToken(
                issuer: _options.Issuer,
                audience: _options.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_options.ExpirationMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
