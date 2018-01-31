namespace Server.Requesthandler
{
	using System.Threading.Tasks;
	using Shared.Commands;
	using Microsoft.EntityFrameworkCore;
	using NServiceBus;
	using NServiceBus.Logging;
	using Server.Data;
	using Shared.DAL;

	public class DeleteCarHandler : IHandleMessages<DeleteCar>
	{
		readonly DbContextOptionsBuilder<CarApiContext> _dbContextOptionsBuilder;
		public DeleteCarHandler(DbContextOptionsBuilder<CarApiContext> dbContextOptionsBuilder)
		{
			_dbContextOptionsBuilder = dbContextOptionsBuilder;
		}

		static ILog log = LogManager.GetLogger<DeleteCarHandler>();

		public Task Handle(DeleteCar message, IMessageHandlerContext context)
		{
			log.Info("Received DeleteCarRequest.");

			using (var unitOfWork = new CarUnitOfWork(new CarApiContext(_dbContextOptionsBuilder.Options)))
			{
				unitOfWork.Cars.Remove(unitOfWork.Cars.Get(message.CarId));
				unitOfWork.Complete();
			}

			// publish an event that a car had been deleted?
			return Task.CompletedTask;
		}
	}
}