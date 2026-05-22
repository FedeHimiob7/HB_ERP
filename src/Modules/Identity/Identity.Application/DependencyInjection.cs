using HB_ERP.SharedKernel.Application.DependencyInjection;
using Identity.Application.Common;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddIdentityApplication(this IServiceCollection services)
        {
            // Solo llamamos al método del SharedKernel y le pasamos el Assembly de este módulo
            services.AddSharedApplicationLayer(typeof(ApplicationAssemblyReference).Assembly);

            // Si este módulo tiene dependencias únicas (ej. un servicio externo), las registras aquí abajo
            // services.AddScoped<IIdentitySpecificService, IdentitySpecificService>();

            return services;
        }
    }
}
