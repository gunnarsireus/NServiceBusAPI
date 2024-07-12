using NServiceBus;
using System.Threading.Tasks;
using NServiceBus.Logging;
using Shared.Requests;
using Shared.Responses;
using Server.Data;
using Microsoft.EntityFrameworkCore;
using Server.DAL;

namespace Server.RequestHandlers
{
	public class UpdateCompanyRequestHandler : IHandleMessages<UpdateCompanyRequest>
	{
    readonly ICompanyRepository _companyRepository;

    public UpdateCompanyRequestHandler(ICompanyRepository caompanyRepository)
    {
      _companyRepository = caompanyRepository;
    }


    static ILog log = LogManager.GetLogger<UpdateCompanyRequest>();

		public async Task Handle(UpdateCompanyRequest message, IMessageHandlerContext context)
		{
			log.Info("Received UpdateCompanyRequest");

      await _companyRepository.UpdateCompanyAsync(message.Company);

      var response = new UpdateCompanyResponse()
      {
        DataId = message.DataId,
        Company = message.Company
      };

      await context.Reply(response);

    }
	}
}