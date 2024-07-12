using Shared.Requests;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Logging;
using Shared.Responses;
using Server.Data;
using Microsoft.EntityFrameworkCore;
using Server.DAL;

namespace Server.RequestHandlers
{
	public class GetCarRequestHandler : IHandleMessages<GetCarRequest>
	{
    readonly ICarRepository _carRepository;

    public GetCarRequestHandler(ICarRepository carRepository)
    {
      _carRepository = carRepository;
    }

    static ILog log = LogManager.GetLogger<GetCarRequestHandler>();

    public async Task Handle(GetCarRequest message, IMessageHandlerContext context)
    {
      log.Info("Received GetCarRequest.");

      var car = await _carRepository.GetCarAsync(message.CarId);

      var response = new GetCarResponse()
      {
        DataId = message.DataId,
        Car = car
      };

      await context.Reply(response);
    }
  }
}