using Shared.Requests;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Logging;
using Server.Data;
using Server.DAL;
using Microsoft.EntityFrameworkCore;

namespace Server.Requesthandler
{
	public class DeleteCarRequestHandler : IHandleMessages<DeleteCarRequest>
	{
		readonly DbContextOptionsBuilder<CarApiContext> _dbContextOptionsBuilder;
		public DeleteCarRequestHandler(DbContextOptionsBuilder<CarApiContext> dbContextOptionsBuilder)
		{
			_dbContextOptionsBuilder = dbContextOptionsBuilder;
		}

		static ILog log = LogManager.GetLogger<DeleteCarRequestHandler>();

		public Task Handle(DeleteCarRequest message, IMessageHandlerContext context)
		{
			log.Info("Received DeleteCarRequest.");
			using (var unitOfWork = new CarUnitOfWork(new CarApiContext(_dbContextOptionsBuilder.Options)))
			{
				unitOfWork.Cars.Remove(unitOfWork.Cars.Get(message.CarId));
				unitOfWork.Complete();
			}

			return Task.CompletedTask;
		}
	}
}