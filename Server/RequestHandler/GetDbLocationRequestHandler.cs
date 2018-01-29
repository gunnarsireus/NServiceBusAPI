using Shared.Requests;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Logging;
using Shared.Response;
using System.IO;

namespace Server.Requesthandler
{
	using System;
	using System.Collections.Generic;
	using Microsoft.Extensions.Configuration;
	using Newtonsoft.Json;
	using Newtonsoft.Json.Linq;

	public class GetDbLocationRequestHandler : IHandleMessages<GetDbLocationRequest>
	{
		static ILog log = LogManager.GetLogger<GetDbLocationRequestHandler>();

		IConfigurationRoot Configuration { get; set; }
		public Task Handle(GetDbLocationRequest message, IMessageHandlerContext context)
		{
			log.Info("Received GetDbLocationRequest.");

			var json = JObject.Parse(File.ReadAllText(Directory.GetCurrentDirectory() + @"\appsettings.json")).ToString();
			var outer = JToken.Parse(json);
			var inner = outer["AppSettings"].Value<JObject>();
			var appSettings = JsonConvert.DeserializeObject<AppSettings>(inner.ToString());
			var response = new GetDbLocationResponse()
			{
				DbLocation = Directory.GetCurrentDirectory() + @"\" + appSettings.DbLocation // "\\App_Data"
			};

			var reply = context.Reply(response);
			return reply;
		}
	}
}