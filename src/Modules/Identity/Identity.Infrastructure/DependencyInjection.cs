using HB_ERP.SharedKernel.Infrastructure.Interceptors;
using Identity.Application;
using Identity.Application.Common.Interfaces;
using Identity.Domain;
using Identity.Domain.Interface;
using Identity.Domain.Repositories;
using Identity.Infrastructure.Authentication;
using Identity.Infrastructure.Persistence;
using Identity.Infrastructure.Persistence.Repositories;
using Identity.Infrastructure.Security;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Identity.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddIdentityInfrastructure(this IServiceCollection services, IConfiguration config)
        {
            services.AddScoped<PublishDomainEventsInterceptor>();

            services.AddDbContext<IdentityDbContext>((sp, options) =>
            {

                options.UseSqlServer(config.GetConnectionString("DefaultConnection"),
                    sqlOptions =>
                    {
                        sqlOptions.MigrationsAssembly(typeof(IdentityDbContext).Assembly.FullName);
                        sqlOptions.MigrationsHistoryTable("__EFMigrationsHistory", "dbo");
                    });

                options.AddInterceptors(sp.GetRequiredService<PublishDomainEventsInterceptor>());
            });

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();

            services.AddScoped<IIdentityUnitOfWork, EfUnitOfWork>();
            services.AddScoped<IUserEmailUniquenessChecker, UserEmailUniquenessChecker>();
            services.AddScoped<IRoleNameUniquenessChecker, RoleNameUniquenessChecker>();
            services.AddScoped<ISystemActionRepository, SystemActionRepository>();

            services.AddScoped<IPasswordHasher, Pbkdf2PasswordHasher>();
            services.AddScoped<IJwtTokenService, JwtTokenService>();
            services.Configure<JwtOptions>(config.GetSection("JwtOptions"));

            var secretKey = config["JwtOptions:Secret"];

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = config["JwtOptions:Issuer"],
                    ValidAudience = config["JwtOptions:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!))
                };
            });


            return services;
        }
    }
}
