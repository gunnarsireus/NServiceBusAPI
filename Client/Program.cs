using Client.DAL;
using Client.Filters;
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
using Microsoft.OpenApi.Models;
using NServiceBus;
using Shared.Particular;
using System;
using System.Globalization;
using System.Threading.Tasks;

namespace Client
{
    public class Program
  {
    public static async Task Main(string[] args)
    {     
      CultureInfo.CurrentUICulture = new CultureInfo("en-US");

      var builder = WebApplication.CreateBuilder(args);

      builder.Configuration
        .AddJsonFile("appsettings.json");

      builder.Logging.AddConsole();

      builder.Host.UseNServiceBus(ctx =>
      {
        string endpointName = "NServiceBusCore.Client";

        var endpointConfiguration = new EndpointConfiguration(endpointName);

        endpointConfiguration.ApplyEndpointConfiguration(
                  ctx.Configuration.GetConnectionString("NServiceBusTransport"),
                  EndpointMappings.MessageEndpointMappings());

        return endpointConfiguration;
      });


      builder.Services.AddDbContext<ApplicationDbContext>(options =>
          options.UseSqlServer(builder.Configuration.GetConnectionString("ASPNETDbConnection")));


      builder.Services.AddTransient<ApplicationDbContext>();
      builder.Services.AddTransient<IdentityDbContext<ApplicationUser>>();

      builder.Services
          .AddIdentity<ApplicationUser, IdentityRole>()
          .AddEntityFrameworkStores<ApplicationDbContext>()
          .AddDefaultTokenProviders();

      builder.Services.AddTransient<IEmailSender, EmailSender>();

      builder.Services.AddControllersWithViews();
      builder.Services.AddSwaggerGen(c =>
      {
          c.SwaggerDoc("v1", new OpenApiInfo { Title = "CarApi", Version = "v1" });
          c.DocumentFilter<CustomDocumentFilter>();  // Register the custom document filter
      });

      builder.Services.AddAuthentication();
      builder.Services.AddAuthorization();

      var app = builder.Build();
      using (var scope = app.Services.GetService<IServiceScopeFactory>().CreateScope())
      {
        scope.ServiceProvider.GetRequiredService<ApplicationDbContext>().Database.EnsureCreated();
      }


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
  
      app.MapControllers();

      app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

      app.UseSwagger();
      app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                c.RoutePrefix = "swagger";
            });


      await app.RunAsync().ConfigureAwait(false);
    }
  }
}