using HB_ERP.SharedKernel.Application.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Inventory.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInventoryApplication(this IServiceCollection services)
        {
            services.AddSharedApplicationLayer(typeof(ApplicationAssemblyReference).Assembly);
            return services;
        }
    }
}
