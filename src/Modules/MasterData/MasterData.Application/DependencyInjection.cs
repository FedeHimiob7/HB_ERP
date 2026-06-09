using HB_ERP.SharedKernel.Application.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterData.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddMasterDataApplication(this IServiceCollection services)
        {
            services.AddSharedApplicationLayer(typeof(ApplicationAssemblyReference).Assembly);
            // Si este módulo tiene dependencias únicas (ej. un servicio externo), las registras aquí abajo
            // services.AddScoped<IIdentitySpecificService, IdentitySpecificService>();
            return services;
        }
    }
}
