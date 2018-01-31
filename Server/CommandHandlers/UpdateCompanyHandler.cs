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

	public class UpdateCompanyHandler : IHandleMessages<UpdateCompany>
	{
		readonly DbContextOptionsBuilder<CarApiContext> _dbContextOptionsBuilder;
		public UpdateCompanyHandler(DbContextOptionsBuilder<CarApiContext> dbContextOptionsBuilder)
		{
			_dbContextOptionsBuilder = dbContextOptionsBuilder;
		}
		static ILog log = LogManager.GetLogger<UpdateCompany>();

		public Task Handle(UpdateCompany message, IMessageHandlerContext context)
		{
			log.Info("Received UpdateCompanyRequest");

			var company = new Company(message.Id);
			// TODO: map object and massege

			using (var unitOfWork = new CarUnitOfWork(new CarApiContext(_dbContextOptionsBuilder.Options)))
			{
				unitOfWork.Companies.Update(company);
				unitOfWork.Complete();
			}

			return Task.CompletedTask;

		}
	}
}