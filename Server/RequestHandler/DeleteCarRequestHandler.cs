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
	public class DeleteCarRequestHandler : IHandleMessages<DeleteCarRequest>
	{
    readonly ICarRepository _carRepository;

    public DeleteCarRequestHandler(ICarRepository carRepository)
    {
      _carRepository = carRepository;
    }

    static ILog log = LogManager.GetLogger<DeleteCarRequestHandler>();

    public async Task Handle(DeleteCarRequest message, IMessageHandlerContext context)
    {
      log.Info("Received DeleteCarRequest.");
      
      await _carRepository.RemoveCarAsync(message.CarId);

      var response = new DeleteCarResponse()
      {
        DataId = message.DataId,
      };

      await context.Reply(response);
    }
  }
}