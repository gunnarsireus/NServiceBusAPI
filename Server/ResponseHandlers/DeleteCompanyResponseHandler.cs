using NServiceBus;
using NServiceBus.Logging;
using Server.Data;
using Shared.Responses;
using System.Threading.Tasks;

namespace Server.ResponseHandlers
{
  public class DeleteCompanyResponseHandler : IHandleMessages<DeleteCompanyResponse>
	{
    static ILog log = LogManager.GetLogger<GetCompanyResponse>();

    public Task Handle(DeleteCompanyResponse message, IMessageHandlerContext context)
    {
      log.Info("Received DeleteCompanyResponse.");
      return Task.CompletedTask;
    }
  }
}