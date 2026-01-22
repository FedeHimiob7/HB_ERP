using Identity.Application.Common.Interfaces;
using Identity.Domain;
using Identity.Domain.Interface;
using Identity.Domain.Repositories;
using Identity.Infrastructure.Authentication;
using Identity.Infrastructure.Persistence;
using Identity.Infrastructure.Persistence.Repositories;
using Identity.Infrastructure.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddIdentityInfrastructure(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<IdentityDbContext>(options =>
                options.UseSqlServer(config.GetConnectionString("DefaultConnection"),
                sqlOptions =>
                {
                        sqlOptions.MigrationsAssembly(typeof(IdentityDbContext).Assembly.FullName);
                        sqlOptions.MigrationsHistoryTable("__EFMigrationsHistory", "dbo");
                })
            ); 

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();

            services.AddScoped<IUnitOfWork, EfUnitOfWork>();
            services.AddScoped<IUserEmailUniquenessChecker, UserEmailUniquenessChecker>();
            services.AddScoped<IRoleNameUniquenessChecker, RoleNameUniquenessChecker>();

            services.AddScoped<IPasswordHasher, Pbkdf2PasswordHasher>();
            services.AddScoped<IJwtTokenService, JwtTokenService>();

            return services;
        }
    }
}
