using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NServiceBus;
using Server.DAL;
using Server.Data;
using Server.RequestHandlers;
using Server.ResponseHandlers;
using Shared.Particular;
using System.Globalization;
using System.Threading.Tasks;

namespace Server
{
    internal class Program
    {
        public static async Task Main(string[] args)
        {
            CultureInfo.CurrentUICulture = new CultureInfo("en-US");
            var builder = new ConfigurationBuilder()
              .AddJsonFile("appsettings.json");

            var configuration = builder.Build();
            var host = CreateHostBuilder(args, configuration).Build();

            using (var scope = host.Services.CreateScope())
            {
                var carapicontext = scope.ServiceProvider.GetRequiredService<CarApiContext>();
                carapicontext.EnsureSeedData();

                var nServiceBusDbContext = scope.ServiceProvider.GetRequiredService<NServiceBusDbContext>();
                var database = nServiceBusDbContext.Database;
                await ClearDatabaseTables(database);
            }

            await host.RunAsync();
        }

        public static async Task ClearDatabaseTables(DatabaseFacade database)
        {
            var tables = new[]
            {
            "error",
            "audit",
            "NServiceBus.Client.Delayed",
            "NServiceBus.Server.Delayed",
            "NServiceBus.Client",
            "NServiceBus.Client.1",
            "NServiceBus.Server",
            "NServiceBus.Server.1",
            "SubscriptionRouting"
        };

            foreach (var table in tables)
            {
                // Check if table exists
                var checkTableExistsSql = $"IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '{table}' AND TABLE_SCHEMA = 'dbo' AND TABLE_CATALOG = 'NServiceBusDb') SELECT 1 ELSE SELECT 0";
                var tableExists = await database.ExecuteSqlRawAsync(checkTableExistsSql);

                if (tableExists == 1)
                {
                    var sql = $"DELETE FROM [NServiceBusDb].[dbo].[{table}]";
                    await database.ExecuteSqlRawAsync(sql);
                }
            }
        }

        static IHostBuilder CreateHostBuilder(string[] args, IConfiguration configuration)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureLogging(logging =>
                {
                    logging.AddConsole();
                })
               .UseNServiceBus(ctx =>
               {
                   string endpointName = "NServiceBus.Server";

                   var endpointConfiguration = new EndpointConfiguration(endpointName);

                   endpointConfiguration.ApplyEndpointConfiguration(
                       ctx.Configuration.GetConnectionString("NServiceBusTransport"),
                       EndpointMappings.MessageEndpointMappings());

                   endpointConfiguration.RegisterComponents(registration =>
             {
                     registration.AddTransient<CreateCarRequestHandler>();
                     registration.AddTransient<CreateCompanyRequestHandler>();
                     registration.AddTransient<DeleteCarRequestHandler>();
                     registration.AddTransient<DeleteCompanyRequestHandler>();
                     registration.AddTransient<GetCarRequestHandler>();
                     registration.AddTransient<GetCarsRequestHandler>();
                     registration.AddTransient<GetCompanyRequestHandler>();
                     registration.AddTransient<GetCompaniesRequestHandler>();
                     registration.AddTransient<UpdateCarRequestHandler>();
                     registration.AddTransient<UpdateCompanyRequestHandler>();

                     registration.AddTransient<CreateCarResponseHandler>();
                     registration.AddTransient<CreateCompanyResponseHandler>();
                     registration.AddTransient<DeleteCarResponseHandler>();
                     registration.AddTransient<DeleteCompanyResponseHandler>();
                     registration.AddTransient<GetCarResponseHandler>();
                     registration.AddTransient<GetCarsResponseHandler>();
                     registration.AddTransient<GetCompanyResponseHandler>();
                     registration.AddTransient<GetCompaniesResponseHandler>();
                     registration.AddTransient<UpdateCarResponseHandler>();
                     registration.AddTransient<UpdateCompanyResponseHandler>();
                 });

                   return endpointConfiguration;
               })
               .ConfigureServices(services =>
               {
                   services.AddDbContext<CarApiContext>(options =>
                 options.UseSqlServer(configuration.GetConnectionString("CarApiConnection")));

                   services.AddDbContext<NServiceBusDbContext>(options =>
                          options.UseSqlServer(configuration.GetConnectionString("NServiceBusTransport")));

                   services.AddTransient<ICarRepository, CarRepository>();
                   services.AddTransient<ICompanyRepository, CompanyRepository>();
               });
        }
    }
}

