using HB_ERP.SharedKernel.Application.Security;
using HB_ERP.SharedKernel.Domain.Primitives;
using HB_ERP.SharedKernel.Infrastructure.Interceptors;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HB_ERP.SharedKernel.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddSharedKernelInfrastructure(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddScoped<ICurrentUserProvider, CurrentUserProvider>();
            services.AddScoped<UpdateAuditableEntitiesInterceptor>();
            services.AddScoped<PublishDomainEventsInterceptor>();

            return services;
        }
    }
}
