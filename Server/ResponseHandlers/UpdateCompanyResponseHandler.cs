using NServiceBus;
using NServiceBus.Logging;
using Server.Data;
using Shared.Responses;
using System.Threading.Tasks;

namespace Server.ResponseHandlers
{
  public class UpdateCompanyResponseHandler : IHandleMessages<UpdateCompanyResponse>
	{
    private readonly ICompanyRepository _companyRepository;

    public UpdateCompanyResponseHandler(ICompanyRepository caompanyRepository)
    {
      _companyRepository = caompanyRepository;
    }


    static ILog log = LogManager.GetLogger<UpdateCompanyResponse>();

		public Task Handle(UpdateCompanyResponse message, IMessageHandlerContext context)
		{
			log.Info("Received UpdateCompanyResponse");
      return Task.CompletedTask;
    }
	}
}