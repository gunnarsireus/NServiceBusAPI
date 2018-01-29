using Shared.Requests;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Logging;
using Shared.Response;

namespace Server.Requesthandler
{
	using System.IO;

	public class GetDbLocationsRequestHandler : IHandleMessages<GetDbLocationsRequest>
	{
		static ILog log = LogManager.GetLogger<GetDbLocationsRequestHandler>();

		public Task Handle(GetDbLocationsRequest message, IMessageHandlerContext context)
		{
			log.Info("Received GetDbRequest.");

			var response = new GetDbLocationsResponse()
			{
				DbLocation = Directory.GetCurrentDirectory() + "\\App_Data"
			};

			var reply = context.Reply(response);
			return reply;
		}
	}
}