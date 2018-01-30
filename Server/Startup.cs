using System;
using Microsoft.Extensions.DependencyInjection;
using NServiceBus;
using Microsoft.EntityFrameworkCore;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Shared.DAL;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace Server
{
	public class Startup
	{
		public Startup(IHostingEnvironment env)
		{
			var builder = new ConfigurationBuilder()
				.SetBasePath(env.ContentRootPath)
				.AddJsonFile("appsettings.json")
				.AddEnvironmentVariables();
			Configuration = builder.Build();
		}

		IContainer Container { get; set; }
		IConfigurationRoot Configuration { get; set; }

		//private readonly IOptions<AppSettings> _appSettings;

		// This method gets called by the runtime. Use this method to add services to the container.

		public IServiceProvider ConfigureServices(IServiceCollection services)
		{
			var dbContextOptionsBuilder = new DbContextOptionsBuilder<CarApiContext>();
			dbContextOptionsBuilder.UseSqlite("DataSource=" + Configuration["AppSettings:DbLocation"] + "/Car.db");

			using (var context = new CarApiContext(dbContextOptionsBuilder.Options))
			{
				context.Database.EnsureCreated();
				context.EnsureSeedData();
			}

			var builder = new ContainerBuilder();
			builder.Populate(services);
			builder.RegisterType<DbContextOptionsBuilder<CarApiContext>>().AsSelf().WithParameter("dbContextOptionsBuilder", dbContextOptionsBuilder);

			IEndpointInstance endpoint = null;
			builder.Register(c => endpoint)
				.As<IEndpointInstance>()
				.SingleInstance();

			Container = builder.Build();

			var endpointConfiguration = new EndpointConfiguration("NServiceBusCore.Server");

			endpointConfiguration.UsePersistence<LearningPersistence>();

			var transport = endpointConfiguration.UseTransport<LearningTransport>();

			endpointConfiguration.Conventions().DefiningCommandsAs(t =>
					t.Namespace != null && t.Namespace.StartsWith("Messages") &&
					(t.Namespace.EndsWith("Commands")))
				.DefiningEventsAs(t =>
					t.Namespace != null && t.Namespace.StartsWith("Messages") &&
					t.Namespace.EndsWith("Events"));

			endpointConfiguration.UseContainer<AutofacBuilder>(
				customizations: customizations =>
				{
					customizations.ExistingLifetimeScope(Container);
				});

			Endpoint.Start(endpointConfiguration).GetAwaiter().GetResult();

			return new AutofacServiceProvider(Container);
		}

		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IApplicationLifetime appLifetime)
		{
			loggerFactory.AddConsole(Configuration.GetSection("Logging"));
			loggerFactory.AddDebug();
			appLifetime.ApplicationStopped.Register(() => Container.Dispose());
		}
	}
}