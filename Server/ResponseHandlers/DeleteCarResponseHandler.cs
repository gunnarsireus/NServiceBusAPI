using NServiceBus;
using NServiceBus.Logging;
using Server.Data;
using Shared.Responses;
using System.Threading.Tasks;

namespace Server.ResponseHandlers
{
  public class DeleteCarResponseHandler : IHandleMessages<DeleteCarResponse>
	{
    private readonly ICarRepository _carRepository;

    public DeleteCarResponseHandler(ICarRepository carRepository)
    {
      _carRepository = carRepository;
    }

    static ILog log = LogManager.GetLogger<DeleteCarResponseHandler>();

    public Task Handle(DeleteCarResponse message, IMessageHandlerContext context)
    {
      log.Info("Received DeleteCarResponse.");
      return Task.CompletedTask;
    }
  }
}