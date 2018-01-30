namespace Server.CommandHandlers
{
	using System.Threading.Tasks;
	using Messages.Commands;
	using Microsoft.EntityFrameworkCore;
	using NServiceBus;
	using NServiceBus.Logging;
	using Server.Data;
	using Shared.DAL;
	using Shared.Models;

	public class CreateCarHandler : IHandleMessages<CreateCar>
	{
		readonly DbContextOptionsBuilder<CarApiContext> _dbContextOptionsBuilder;
		public CreateCarHandler(DbContextOptionsBuilder<CarApiContext> dbContextOptionsBuilder)
		{
			_dbContextOptionsBuilder = dbContextOptionsBuilder;
		}

		static ILog log = LogManager.GetLogger<CreateCarHandler>();

		public Task Handle(CreateCar message, IMessageHandlerContext context)
		{
			log.Info("Received CreateCar.");

			var car = new Car(message.Id);
			// TODO: map object and massege

			using (var unitOfWork = new CarUnitOfWork(new CarApiContext(_dbContextOptionsBuilder.Options)))
			{
				unitOfWork.Cars.Add(car);
				unitOfWork.Complete();
			}

			// Publish an event that a car was created?
			return Task.CompletedTask;
		}
	}
}