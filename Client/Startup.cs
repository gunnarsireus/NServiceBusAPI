using Client.DAL;
using Client.Models;
using Client.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NServiceBus;
using NServiceBus.Transport.SqlServer;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Client
{
  public class Startup
	{
    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }
		public IConfiguration Configuration { get; }
    IEndpointInstance EndpointInstance { get; set; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
      ConfigureServicesAsync(services);

      var endpointConfiguration = new EndpointConfiguration("NServiceBusCore.Client");
      endpointConfiguration.SendFailedMessagesTo("error");
      endpointConfiguration.EnableInstallers();

      var connectionString = Configuration.GetConnectionString("NServiceBusTransport");
      var transport = new SqlServerTransport(connectionString)
      {
        Subscriptions =
            {
                SubscriptionTableName = new SubscriptionTableName(
                        table: "Subscriptions",
                        schema: "dbo")
            },
        TransportTransactionMode = TransportTransactionMode.SendsAtomicWithReceive
      };

      endpointConfiguration.UseSerialization<SystemJsonSerializer>();
      var routing = endpointConfiguration.UseTransport(transport);

      endpointConfiguration.MakeInstanceUniquelyAddressable("1");

      endpointConfiguration.EnableCallbacks();
      EndpointInstance = Endpoint.Start(endpointConfiguration).ConfigureAwait(false).GetAwaiter().GetResult();

      services.AddSingleton(EndpointInstance);
		}

		void ConfigureServicesAsync(IServiceCollection services)
		{
      var dbFilePath = Path.Combine(Path.Combine(AppContext.BaseDirectory, "App_Data"), "AspNet.db");
      services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlite($"Data Source={dbFilePath}"));

      services.AddTransient<ApplicationDbContext>();
      services.AddTransient<IdentityDbContext<ApplicationUser>>();

      services.AddIdentity<ApplicationUser, IdentityRole>()
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();


      services.AddTransient<IEmailSender, EmailSender>();

      services.AddMvc();

      services.AddAuthentication();

      services.AddAuthorization();
    }


		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }
      else
      {
        app.UseExceptionHandler("/Home/Error");
      }

      app.UseStaticFiles();
     
      app.UseRouting();

      app.UseAuthentication(); 

      app.UseAuthorization();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");
      });
    }
  }
}