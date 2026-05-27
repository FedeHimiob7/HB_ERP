using HB_ERP.SharedKernel.Application.Interfaces;
using HB_ERP.SharedKernel.Infrastructure;
using HB_ERP.SharedKernel.Infrastructure.Interceptors;
using MassTransit;
using MasterData.Application;
using MasterData.Domain.Repositories;
using MasterData.Infrastructure.Persistence;
using MasterData.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MasterData.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddMasterDataInfrastructure(
                                    this IServiceCollection services,
                                    IConfiguration configuration)
        {
            // 1. Registramos el interceptor explícitamente para este módulo
            services.AddScoped<UpdateAuditableEntitiesInterceptor>();
            services.AddScoped<PublishDomainEventsInterceptor>();

            services.AddDbContext<MasterDataDbContext>((serviceProvider, options) =>
            {
                var connectionString = configuration.GetConnectionString("DefaultConnection")
                    ?? "Server=DESKTOP-QAB7V8I;Database=HB_ERP;Integrated Security=True;Encrypt=False";

                options.UseSqlServer(connectionString, sqlOptions =>
                {
                    sqlOptions.MigrationsAssembly(typeof(MasterDataDbContext).Assembly.FullName);
                    sqlOptions.MigrationsHistoryTable("__EFMigrationsHistory", "MasterData");
                });

                // NATIVO EF CORE: Le decimos que busque el interceptor en el Scope actual de la petición automáticamente
                options.AddInterceptors(
                            serviceProvider.GetRequiredService<UpdateAuditableEntitiesInterceptor>(),
                            serviceProvider.GetRequiredService<PublishDomainEventsInterceptor>()
    );
            });

            services.AddScoped<IOutboxRepository, OutboxRepository>();
            services.AddScoped<IUnitOfWork, MasterDataEfUnitOfWork>();     


            services.AddScoped<ICurrencyRepository, CurrencyRepository>();
            services.AddScoped<IProductServiceLineRepository, ProductServiceLineRepository>();                   
            services.AddScoped<ICountryRepository, CountryRepository>();
            services.AddScoped<IStateRepository, StateRepository>();

            return services;
        }
    }
}
