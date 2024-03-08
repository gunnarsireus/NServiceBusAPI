using NServiceBus;
using NServiceBus.Logging;
using Server.Data;
using Shared.Responses;
using System.Threading.Tasks;

namespace Server.ResponseHandlers
{
  public class CreateCarResponseHandler : IHandleMessages<CreateCarResponse>
  {
    private readonly ICarRepository _carRepository;

    public CreateCarResponseHandler(ICarRepository carRepository)
    {
      _carRepository = carRepository;
    }

    static ILog log = LogManager.GetLogger<CreateCarResponseHandler>();

		public Task Handle(CreateCarResponse message, IMessageHandlerContext context)
		{
			log.Info("Received CreateCarResponse.");
      return Task.CompletedTask;
    }
	}
}