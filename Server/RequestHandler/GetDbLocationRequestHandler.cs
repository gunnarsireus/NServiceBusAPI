using Shared.Requests;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Logging;
using Shared.Response;
using System.IO;

namespace Server.Requesthandler
{
	public class GetDbLocationRequestHandler : IHandleMessages<GetDbLocationRequest>
	{
		static ILog log = LogManager.GetLogger<GetDbLocationRequestHandler>();
		public Task Handle(GetDbLocationRequest message, IMessageHandlerContext context)
		{
			log.Info("Received GetDbLocationRequest.");

			var response = new GetDbLocationResponse()
			{
				DbLocation = Directory.GetCurrentDirectory() + "\\App_Data"
			};

			var reply = context.Reply(response);
			return reply;
		}
	}
}