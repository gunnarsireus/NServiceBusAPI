using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NServiceBus;
using NServiceBus.Transport.SqlServer;
using Server.DAL;
using Server.Data;
using Server.RequestHandlers;
using Server.ResponseHandlers;
using Shared;
using Shared.Requests;
using System;
using System.ComponentModel;
using System.IO;

namespace Server
{
  public class Startup
	{
    IContainer ApplicationContainer { get; set; }
    IConfiguration Configuration { get; }

    IEndpointInstance EndpointInstance { get; set; }

    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
      var dbFilePath = Path.Combine(Path.Combine(AppContext.BaseDirectory, "App_Data"), "Car.db");
      services.AddDbContext<CarApiContext>(options =>
          options.UseSqlite($"Data Source={dbFilePath}"));

      services.AddTransient<CarApiContext>();
      services.AddTransient<ICarRepository, CarRepository>();
      services.AddTransient<ICompanyRepository, CompanyRepository>();
      services.AddTransient<CreateCarRequestHandler>();
      services.AddTransient<CreateCompanyRequestHandler>();
      services.AddTransient<DeleteCarRequestHandler>();
      services.AddTransient<DeleteCompanyRequestHandler>();
      services.AddTransient<GetCarRequestHandler>();
      services.AddTransient<GetCarsRequestHandler>();
      services.AddTransient<GetCompanyRequestHandler>();
      services.AddTransient<GetCompaniesRequestHandler>();
      services.AddTransient<UpdateCarRequestHandler>();
      services.AddTransient<UpdateCompanyRequestHandler>();

      services.AddTransient<CreateCarResponseHandler>();
      services.AddTransient<CreateCompanyResponseHandler>();
      services.AddTransient<DeleteCarResponseHandler>();
      services.AddTransient<DeleteCompanyResponseHandler>();
      services.AddTransient<GetCarResponseHandler>();
      services.AddTransient<GetCarsResponseHandler>();
      services.AddTransient<GetCompanyResponseHandler>();
      services.AddTransient<GetCompaniesResponseHandler>();
      services.AddTransient<UpdateCarResponseHandler>();
      services.AddTransient<UpdateCompanyResponseHandler>();

      CarApiExtensions.InitSqLiteDb();
      
      // Configure NServiceBus endpoint
      var endpointConfiguration = new EndpointConfiguration("NServiceBusCore.Server");
      endpointConfiguration.SendFailedMessagesTo("error");
      endpointConfiguration.AuditProcessedMessagesTo("audit");
      endpointConfiguration.EnableInstallers();

      var connectionString = Configuration.GetConnectionString("NServiceBusTransport");

      var transport = new SqlServerTransport(connectionString)
      {
        TransportTransactionMode = TransportTransactionMode.SendsAtomicWithReceive,
        Subscriptions =
            {
                CacheInvalidationPeriod = TimeSpan.FromMinutes(1),
                SubscriptionTableName = new SubscriptionTableName(table: "Subscriptions", schema: "dbo")
            }
      };

      endpointConfiguration.UseSerialization<SystemJsonSerializer>();
      var routing = endpointConfiguration.UseTransport(transport);

      endpointConfiguration.MakeInstanceUniquelyAddressable("1");

            EndpointInstance = EndpointWithExternallyManagedContainer.Create(endpointConfiguration, services)
                .Start(services.BuildServiceProvider())
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();

            services.AddSingleton(EndpointInstance);

            _ = EndpointInstance.SendLocal(new GetCarsRequest { });
    }

    public void Configure(IApplicationLifetime appLifetime)
    {
      appLifetime.ApplicationStopped.Register(() => ApplicationContainer.Dispose());
    }
  }
}