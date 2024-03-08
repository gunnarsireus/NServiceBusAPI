using NServiceBus;
using NServiceBus.Logging;
using Server.Data;
using Shared.Responses;
using System.Threading.Tasks;

namespace Server.ResponseHandlers
{
  public class UpdateCarResponseHandler : IHandleMessages<UpdateCarResponse>
	{
    private readonly ICarRepository _carRepository;

    public UpdateCarResponseHandler(ICarRepository carRepository)
    {
      _carRepository = carRepository;
    }

    static ILog log = LogManager.GetLogger<UpdateCarResponseHandler>();

    public async Task Handle(UpdateCarResponse message, IMessageHandlerContext context)
    {
      log.Info("Received UpdateCarResponse.");

      //await _carRepository.UpdateCarAsync(message.Car);

      //var response = new UpdateCarResponse()
      //{
      //  DataId = message.DataId,
      //  Car = message.Car
      //};

      //await context.Reply(response);
    }
  }
}