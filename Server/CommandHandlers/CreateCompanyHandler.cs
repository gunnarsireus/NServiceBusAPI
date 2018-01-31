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

	public class CreateCompanyHandler : IHandleMessages<CreateCompany>
	{
		readonly DbContextOptionsBuilder<CarApiContext> _dbContextOptionsBuilder;
		public CreateCompanyHandler(DbContextOptionsBuilder<CarApiContext> dbContextOptionsBuilder)
		{
			_dbContextOptionsBuilder = dbContextOptionsBuilder;
		}

		static ILog log = LogManager.GetLogger<CreateCompany>();

		public Task Handle(CreateCompany message, IMessageHandlerContext context)
		{
			log.Info("Received CreateCompanyRequest");

			var company = new Company(message.Id);
			// TODO: map object and massege

			using (var unitOfWork = new CarUnitOfWork(new CarApiContext(_dbContextOptionsBuilder.Options)))
			{
				unitOfWork.Companies.Add(company);
				unitOfWork.Complete();
			}

			// publish an event that a company was created?
			return Task.CompletedTask;
		}
	}
}