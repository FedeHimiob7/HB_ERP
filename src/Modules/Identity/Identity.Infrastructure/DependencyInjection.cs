using Identity.Application.Common.Interfaces;
using Identity.Domain;
using Identity.Domain.Interface;
using Identity.Domain.Repositories;
using Identity.Infrastructure.Authentication;
using Identity.Infrastructure.Persistence;
using Identity.Infrastructure.Persistence.Interceptors;
using Identity.Infrastructure.Persistence.Repositories;
using Identity.Infrastructure.Security;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
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

            services.AddScoped<IUnitOfWork, EfUnitOfWork>();
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

            services.AddMassTransit(x =>
            {
                // 1. Configuramos el Outbox para Entity Framework Core
                x.AddEntityFrameworkOutbox<IdentityDbContext>(o =>
                {
                    // Le decimos que use SQL Server
                    o.UseSqlServer();

                    // ESTO ES EL WORKER: Le decimos a MassTransit que envíe los mensajes
                    // guardados en la tabla hacia RabbitMQ en segundo plano automáticamente.
                    o.UseBusOutbox();
                });

                // 2. Configuramos la conexión a RabbitMQ
                x.UsingRabbitMq((context, cfg) =>
                {
                    // Configuración por defecto de RabbitMQ en local (Docker)
                    cfg.Host("localhost", "/", h =>
                    {
                        h.Username("guest");
                        h.Password("guest");
                    });

                    // Esto auto-configura las colas basado en los consumidores que creemos luego
                    cfg.ConfigureEndpoints(context);
                });
            });

            return services;
        }
    }
}
