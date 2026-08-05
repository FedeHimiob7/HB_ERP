using MasterData.Application.Interfaces;
using HB_ERP.SharedKernel.Infrastructure;
using HB_ERP.SharedKernel.Infrastructure.Interceptors;
using MassTransit;
using MasterData.Application;
using MasterData.Domain.Repositories;
using MasterData.Infrastructure.BackgroundServices;
using MasterData.Infrastructure.Persistence;
using MasterData.Infrastructure.Persistence.Repositories;
using MasterData.Infrastructure.Services;
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
            services.AddScoped<IMasterDataUnitOfWork, MasterDataEfUnitOfWork>();     


            services.AddScoped<ICompanyRepository, CompanyRepository>();
            services.AddScoped<IBranchRepository, BranchRepository>();
            services.AddScoped<IFiscalTerminalRepository, FiscalTerminalRepository>();
            services.AddScoped<ICurrencyRepository, CurrencyRepository>();
            services.AddScoped<IProductServiceLineRepository, ProductServiceLineRepository>();
            services.AddScoped<ICountryRepository, CountryRepository>();
            services.AddScoped<IStateRepository, StateRepository>();
            services.AddScoped<IUnitRepository, UnitRepository>();
            services.AddScoped<ITaxRepository, TaxRepository>();
            services.AddScoped<IFiscalTaxRateRepository, FiscalTaxRateRepository>();
            services.AddScoped<ICityRepository, CityRepository>();
            services.AddScoped<IExchangeRateRepository, ExchangeRateRepository>();
            services.AddScoped<IBCVRateScrapingService, BCVRateScrapingService>();
            services.AddHttpClient("BCV", client =>
            {
                client.DefaultRequestHeaders.Add("User-Agent",
                    "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 Chrome/124.0.0.0 Safari/537.36");
                client.Timeout = TimeSpan.FromSeconds(30);
            });

            services.AddHostedService<BCVRateSyncWorker>();

            return services;
        }
    }
}
