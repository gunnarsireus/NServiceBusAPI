using NServiceBus;
using NServiceBus.Logging;
using Server.Data;
using Shared.Responses;
using System.Threading.Tasks;

namespace Server.ResponseHandlers
{
  public class CreateCarResponseHandler : IHandleMessages<CreateCarResponse>
  {
    static ILog log = LogManager.GetLogger<CreateCarResponseHandler>();

		public Task Handle(CreateCarResponse message, IMessageHandlerContext context)
		{
			log.Info("Received CreateCarResponse.");
      return Task.CompletedTask;
    }
	}
}