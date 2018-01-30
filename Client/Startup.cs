namespace Client
{
    using System.IO;
    using Client.Models;
    using Client.Services;
    using Messages.Commands;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using NServiceBus;
    using Shared.DAL;

    public class Startup
    {
        IEndpointInstance EndpointInstance { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var endpointConfiguration = new EndpointConfiguration("NServiceBusCore.Client");
            var transport = endpointConfiguration.UseTransport<LearningTransport>();
            endpointConfiguration.UsePersistence<LearningPersistence>();
	        endpointConfiguration.UsePersistence<LearningPersistence>();

            endpointConfiguration.UseTransport<LearningTransport>()
                .Routing().RouteToEndpoint(assembly: typeof(CreateCar).Assembly, destination: "NServiceBusCore.Server");

			endpointConfiguration.Conventions().DefiningCommandsAs(t =>
                    t.Namespace != null && t.Namespace.StartsWith("Messages") &&
                    (t.Namespace.EndsWith("Commands")))
                .DefiningEventsAs(t =>
                    t.Namespace != null && t.Namespace.StartsWith("Messages") &&
                    t.Namespace.EndsWith("Events"));

            EndpointInstance = Endpoint.Start(endpointConfiguration).GetAwaiter().GetResult();

            services.AddSingleton(EndpointInstance);

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            //// Add application services.
            services.AddTransient<IEmailSender, EmailSender>();
            services.AddMvc();

           // var dblocation = Directory.GetCurrentDirectory() + @"\Server\AppData";

            var dblocation = Directory.GetParent(Directory.GetCurrentDirectory()) + @"\Server\App_Data";

            services.AddDbContext<CarApiContext>(options => options.UseSqlite("Data Source=" + dblocation + "/Car.db"));

            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite("Data Source=" + dblocation + "/AspNet.db"));
        }
        
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddDebug();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    "default",
                    "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}