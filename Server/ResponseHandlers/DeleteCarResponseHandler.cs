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

    public async Task Handle(DeleteCarResponse message, IMessageHandlerContext context)
    {
      log.Info("Received DeleteCarResponse.");
      
      //await _carRepository.RemoveCarAsync(message.CarId);

      //var response = new DeleteCarResponse()
      //{
      //  DataId = message.DataId,
      //};

      //await context.Reply(response);
    }
  }
}