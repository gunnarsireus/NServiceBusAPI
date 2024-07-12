using NServiceBus;
using NServiceBus.Logging;
using Server.Data;
using Shared.Responses;
using System.Threading.Tasks;

namespace Server.ResponseHandlers
{
  public class GetCarsResponseHandler : IHandleMessages<GetCarsResponse>
	{

    static ILog log = LogManager.GetLogger<GetCarsResponseHandler>();

    public Task Handle(GetCarsResponse message, IMessageHandlerContext context)
    {
      log.Info("Received GetCarsResponse.");
      return Task.CompletedTask;
    }
	}
}