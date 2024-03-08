using NServiceBus;
using NServiceBus.Logging;
using Server.Data;
using Shared.Responses;
using System.Threading.Tasks;

namespace Server.ResponseHandlers
{
  public class GetCarResponseHandler : IHandleMessages<GetCarResponse>
	{
    static ILog log = LogManager.GetLogger<GetCarResponseHandler>();

    public Task Handle(GetCarResponse message, IMessageHandlerContext context)
    {
      log.Info("Received GetCarResponse.");
      return Task.CompletedTask;
    }
  }
}