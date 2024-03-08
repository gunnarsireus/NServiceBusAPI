using NServiceBus;
using NServiceBus.Logging;
using Server.Data;
using Shared.Responses;
using System.Threading.Tasks;

namespace Server.ResponseHandlers
{
  public class UpdateCarResponseHandler : IHandleMessages<UpdateCarResponse>
	{
  
    static ILog log = LogManager.GetLogger<UpdateCarResponseHandler>();

    public Task Handle(UpdateCarResponse message, IMessageHandlerContext context)
    {
      log.Info("Received UpdateCarResponse.");
      return Task.CompletedTask;
    }
  }
}