using FluentValidation;
using HB_ERP.SharedKernel.Application.Behaviors;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HB_ERP.SharedKernel.Application.DependencyInjection
{
    public static class SharedApplicationDependencyInjection
    {
        // Este método recibe el IServiceCollection y el Ensamblado (Assembly) del módulo que lo está llamando
        public static IServiceCollection AddSharedApplicationLayer(this IServiceCollection services, Assembly moduleAssembly)
        {
            // 1. Registra MediatR para el módulo específico
            services.AddMediatR(config => {
                config.RegisterServicesFromAssembly(moduleAssembly);
            });

            // 2. Registra el ValidationBehavior universal (que ya vive aquí en el SharedKernel)
            services.AddScoped(
                typeof(IPipelineBehavior<,>),
                typeof(ValidationBehavior<,>)
            );

            // 3. Registra los validadores del módulo específico
            services.AddValidatorsFromAssembly(moduleAssembly);

            return services;
        }
    }
}
