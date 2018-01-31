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

	public class UpdateCarHandler : IHandleMessages<UpdateCar>
	{
		readonly DbContextOptionsBuilder<CarApiContext> _dbContextOptionsBuilder;
		// What does update mean?
		public UpdateCarHandler(DbContextOptionsBuilder<CarApiContext> dbContextOptionsBuilder)
		{
			_dbContextOptionsBuilder = dbContextOptionsBuilder;
		}

		static ILog log = LogManager.GetLogger<UpdateCarHandler>();

		public Task Handle(UpdateCar message, IMessageHandlerContext context)
		{
			log.Info("Received UpdateCar.");

			var car = new Car(message.Id);
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