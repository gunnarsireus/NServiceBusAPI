namespace Server.Requesthandler
{
	using System.Threading.Tasks;
	using Shared.Commands;
	using Microsoft.EntityFrameworkCore;
	using NServiceBus;
	using NServiceBus.Logging;
	using Server.Data;
	using Shared.DAL;
	using Shared.Models;

	public class UpdateCarOnlineStatusHandler : IHandleMessages<UpdateCarOnlineStatus>
	{
		readonly DbContextOptionsBuilder<CarApiContext> _dbContextOptionsBuilder;
		// What does update mean?
		public UpdateCarOnlineStatusHandler(DbContextOptionsBuilder<CarApiContext> dbContextOptionsBuilder)
		{
			_dbContextOptionsBuilder = dbContextOptionsBuilder;
		}

		static ILog log = LogManager.GetLogger<UpdateCarHandler>();

		public Task Handle(UpdateCarOnlineStatus message, IMessageHandlerContext context)
		{
			log.Info("Received UpdateCar.");

			var car = new Car(message.Id);
			car.Online = message.Online;
			// TODO: map object and massege

			using (var unitOfWork = new CarUnitOfWork(new CarApiContext(_dbContextOptionsBuilder.Options)))
			{
				unitOfWork.Cars.Update(car);
				unitOfWork.Complete();
			}

			return Task.CompletedTask;
		}
	}
}