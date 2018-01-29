using Shared.Requests;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Logging;
using Server.DAL;
using Server.Data;
using Microsoft.EntityFrameworkCore;

namespace Server.Requesthandler
{
	public class CreateCarRequestHandler : IHandleMessages<CreateCarRequest>
	{
		readonly DbContextOptionsBuilder<CarApiContext> _dbContextOptionsBuilder;
		public CreateCarRequestHandler(DbContextOptionsBuilder<CarApiContext> dbContextOptionsBuilder)
		{
			_dbContextOptionsBuilder = dbContextOptionsBuilder;
		}

		static ILog log = LogManager.GetLogger<CreateCarRequestHandler>();

		public Task Handle(CreateCarRequest message, IMessageHandlerContext context)
		{
			log.Info("Received CreateCarRequest.");

			using (var unitOfWork = new CarUnitOfWork(new CarApiContext(_dbContextOptionsBuilder.Options)))
			{
				unitOfWork.Cars.Add(message.Car);
				unitOfWork.Complete();
			}

			return Task.CompletedTask;
		}
	}
}