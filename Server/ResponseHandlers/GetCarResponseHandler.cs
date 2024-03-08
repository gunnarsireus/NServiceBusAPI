using NServiceBus;
using NServiceBus.Logging;
using Server.Data;
using Shared.Responses;
using System.Threading.Tasks;

namespace Server.ResponseHandlers
{
  public class GetCarResponseHandler : IHandleMessages<GetCarResponse>
	{
    private readonly ICarRepository _carRepository;

    public GetCarResponseHandler(ICarRepository carRepository)
    {
      _carRepository = carRepository;
    }

    static ILog log = LogManager.GetLogger<GetCarResponseHandler>();

    public async Task Handle(GetCarResponse message, IMessageHandlerContext context)
    {
      log.Info("Received GetCarResponse.");

      //var car = await _carRepository.GetCarAsync(message.CarId);

      //var response = new GetCarResponse()
      //{
      //  DataId = message.DataId,
      //  Car = car
      //};

      //await context.Reply(response);
    }
  }
}