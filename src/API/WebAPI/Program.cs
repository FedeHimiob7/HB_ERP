
using HB_ERP.SharedKernel.Infrastructure;
using Identity.Application;
using Identity.Application.EventHandlers;
using Identity.Infrastructure;
using Identity.Infrastructure.Persistence;
using MassTransit;
using MassTransit.EntityFrameworkCoreIntegration;
using MasterData.Application;
using MasterData.Infrastructure;
using MasterData.Infrastructure.Persistence;
using Serilog;
using WebAPI.Middlewares;

namespace WebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();


            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .Enrich.FromLogContext()
                .CreateLogger();

            try
            {
                Log.Information("Iniciando el servidor HB_ERP...");

                var builder = WebApplication.CreateBuilder(args);


                builder.Host.UseSerilog();

                // Registro de dependencias...
                builder.Services.AddSharedKernelInfrastructure() 
                                .AddPresentation()
                                //Identity
                                .AddIdentityInfrastructure(builder.Configuration)
                                .AddIdentityApplication()
                                //MasterData
                                .AddMasterDataInfrastructure(builder.Configuration)
                                .AddMasterDataApplication();

                builder.Services.AddMassTransit(x =>
                {                    
                    x.AddConsumersFromNamespaceContaining<CurrencyCreatedConsumer>();
                   
                    x.UsingRabbitMq((context, cfg) =>
                    {
                        cfg.Host("localhost", "/", h =>
                        {
                            h.Username("guest");
                            h.Password("guest");
                        });

                        cfg.ConfigureEndpoints(context, new KebabCaseEndpointNameFormatter("hb-erp", false));
                        cfg.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));
                    });
                });

                builder.Services.AddHostedService<MasterDataOutboxPublisher>();
                builder.Services.AddHostedService<IdentityOutboxPublisher>();

                var app = builder.Build();              
                               
                if (app.Environment.IsDevelopment())
                {
                    app.UseSwagger();
                    app.UseSwaggerUI();
                }

                app.UseExceptionHandler("/error");
                app.UseHttpsRedirection();
                app.UseAuthentication();
                app.UseMiddleware<UserLogMiddleware>();
                app.UseAuthorization();
                app.MapControllers();

                app.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "El servidor HB_ERP falló catastróficamente al arrancar.");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
