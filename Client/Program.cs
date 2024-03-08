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
using Microsoft.Extensions.Logging;
using NServiceBus;
using Shared.Particular;
using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;

namespace Client
{
  public class Program
    {
        public static async Task Main(string[] args)
        {
            CultureInfo.CurrentUICulture = new CultureInfo("en-US");

            var builder = WebApplication.CreateBuilder(args);

            builder.Configuration.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            builder.Logging.AddConsole();

            builder.Host.UseNServiceBus(ctx =>
            {
                string endpointName = "NServiceBusCore.Client";

                var endpointConfiguration = new EndpointConfiguration(endpointName);

                endpointConfiguration.ApplyEndpointConfiguration(
                    ctx.Configuration.GetConnectionString("NServiceBusTransport"),
                    endpointName,
                    EndpointMappings.MessageEndpointMappings());

                return endpointConfiguration;
            });

            var dbFilePath = Path.Combine(Path.Combine(AppContext.BaseDirectory, "App_Data"), "AspNet.db");

            builder.Services
                .AddDbContext<ApplicationDbContext>(options => options.UseSqlite($"Data Source={dbFilePath}"));

            builder.Services.AddTransient<ApplicationDbContext>();
            builder.Services.AddTransient<IdentityDbContext<ApplicationUser>>();

            builder.Services
                .AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            builder.Services.AddTransient<IEmailSender, EmailSender>();

            builder.Services.AddMvc();
            builder.Services.AddAuthentication();
            builder.Services.AddAuthorization();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
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
            // app.MapGet("/", () => "{controller=Home}/{action=Index}/{id?}");

            await app.RunAsync().ConfigureAwait(false);
        }
    }
}