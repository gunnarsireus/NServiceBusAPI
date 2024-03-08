using NServiceBus;
using NServiceBus.Logging;
using Server.Data;
using Shared.Responses;
using System.Threading.Tasks;

namespace Server.ResponseHandlers
{
  public class DeleteCarResponseHandler : IHandleMessages<DeleteCarResponse>
	{
    static ILog log = LogManager.GetLogger<DeleteCarResponseHandler>();

    public Task Handle(DeleteCarResponse message, IMessageHandlerContext context)
    {
      log.Info("Received DeleteCarResponse.");
      return Task.CompletedTask;
    }
  }
}