using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
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
using System;
using System.Globalization;
using System.IO;
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
      var app = CreateHostBuilder(args, configuration).Build();

            using (var scope = app.Services.GetService<IServiceScopeFactory>().CreateScope())
            {
                var carapicontext = scope.ServiceProvider.GetRequiredService<CarApiContext>();
                carapicontext.EnsureSeedData();

                var database = scope.ServiceProvider.GetRequiredService<NServiceBusDbContext>().Database;

                var sql = $"delete from [NServiceBusDb].[dbo].[error]";
                await database.ExecuteSqlRawAsync(sql);

                sql = $"delete from [NServiceBusDb].[dbo].[audit]";
                await database.ExecuteSqlRawAsync(sql);

                sql = $"delete from [NServiceBusDb].[dbo].[NServiceBusCore.Client.Delayed]";
                await database.ExecuteSqlRawAsync(sql);

                sql = $"delete from [NServiceBusDb].[dbo].[NServiceBusCore.Server.Delayed]";
                await database.ExecuteSqlRawAsync(sql);

                sql = $"delete from [NServiceBusDb].[dbo].[NServiceBusCore.Client]";
                await database.ExecuteSqlRawAsync(sql);

                sql = $"delete from [NServiceBusDb].[dbo].[NServiceBusCore.Client.1]";
                await database.ExecuteSqlRawAsync(sql);

                sql = $"delete from [NServiceBusDb].[dbo].[NServiceBusCore.Server]";
                await database.ExecuteSqlRawAsync(sql);

                sql = $"delete from [NServiceBusDb].[dbo].[NServiceBusCore.Server.1]";
                await database.ExecuteSqlRawAsync(sql);

                sql = $"delete from [NServiceBusDb].[dbo].[SubscriptionRouting]";
                await database.ExecuteSqlRawAsync(sql);
            }

            await app.RunAsync();
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
           string endpointName = "NServiceBusCore.Server";

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

