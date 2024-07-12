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
	public class UpdateCarRequestHandler : IHandleMessages<UpdateCarRequest>
	{
    readonly ICarRepository _carRepository;

    public UpdateCarRequestHandler(ICarRepository carRepository)
    {
      _carRepository = carRepository;
    }

    static ILog log = LogManager.GetLogger<UpdateCarRequestHandler>();

    public async Task Handle(UpdateCarRequest message, IMessageHandlerContext context)
    {
      log.Info("Received UpdateCarRequest.");

      await _carRepository.UpdateCarAsync(message.Car);

      var response = new UpdateCarResponse()
      {
        DataId = message.DataId,
        Car = message.Car
      };

      await context.Reply(response);
    }
  }
}