using HB_ERP.SharedKernel.Infrastructure.Interceptors;
using Inventory.Application.Interfaces;
using Inventory.Domain.Repositories;
using Inventory.Infrastructure.Persistence;
using Inventory.Infrastructure.Persistence.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Inventory.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInventoryInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddScoped<UpdateAuditableEntitiesInterceptor>();
            services.AddScoped<PublishDomainEventsInterceptor>();

            services.AddDbContext<InventoryDbContext>((serviceProvider, options) =>
            {
                var connectionString = configuration.GetConnectionString("DefaultConnection")
                    ?? "Server=DESKTOP-QAB7V8I;Database=HB_ERP;Integrated Security=True;Encrypt=False";

                options.UseSqlServer(connectionString, sqlOptions =>
                {
                    sqlOptions.MigrationsAssembly(typeof(InventoryDbContext).Assembly.FullName);
                    sqlOptions.MigrationsHistoryTable("__EFMigrationsHistory", "Inventory");
                });

                options.AddInterceptors(
                    serviceProvider.GetRequiredService<UpdateAuditableEntitiesInterceptor>(),
                    serviceProvider.GetRequiredService<PublishDomainEventsInterceptor>());
            });

            // Domain event handlers de este módulo (se agregan aquí cuando se necesiten)
            services.AddMediatR(cfg =>
                cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

            services.AddScoped<IInventoryUnitOfWork, InventoryEfUnitOfWork>();

            services.AddScoped<IProductTypeRepository, ProductTypeRepository>();
            services.AddScoped<IProductBrandRepository, ProductBrandRepository>();
            services.AddScoped<IStorageTypeRepository, StorageTypeRepository>();
            services.AddScoped<IProductCategoryRepository, ProductCategoryRepository>();
            services.AddScoped<IProductSubCategoryRepository, ProductSubCategoryRepository>();
            services.AddScoped<IWarehouseRepository, WarehouseRepository>();

            return services;
        }
    }
}
