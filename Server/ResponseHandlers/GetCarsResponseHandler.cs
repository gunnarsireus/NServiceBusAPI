using NServiceBus;
using NServiceBus.Logging;
using Server.Data;
using Shared.Responses;
using System.Threading.Tasks;

namespace Server.ResponseHandlers
{
  public class GetCarsResponseHandler : IHandleMessages<GetCarsResponse>
	{
    private readonly ICarRepository _carRepository;

    public GetCarsResponseHandler(ICarRepository carRepository)
    {
            _carRepository = carRepository;
    }

    static ILog log = LogManager.GetLogger<GetCarsResponseHandler>();

		public async Task Handle(GetCarsResponse message, IMessageHandlerContext context)
		{
			log.Info("Received GetCarsResponse.");

      //var cars = await _carRepository.GetAllCarsAsync();

      //var response = new GetCarsResponse()
      //{
      //  DataId = message.DataId,
      //  Cars = cars.ToList()
      //};

      //await context.Reply(response);
    }
	}
}