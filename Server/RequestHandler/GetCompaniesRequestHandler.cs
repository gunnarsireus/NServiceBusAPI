using NServiceBus;
using System.Threading.Tasks;
using NServiceBus.Logging;
using Shared.Requests;
using Shared.Responses;
using System.Linq;
using Server.Data;
using Microsoft.EntityFrameworkCore;
using Server.DAL;

namespace Server.RequestHandlers
{
	public class GetCompaniesRequestHandler : IHandleMessages<GetCompaniesRequest>
	{
    readonly ICompanyRepository _companyRepository;

    public GetCompaniesRequestHandler(ICompanyRepository caompanyRepository)
    {
      _companyRepository = caompanyRepository;
    }

    static ILog log = LogManager.GetLogger<GetCompaniesRequest>();

		public async Task Handle(GetCompaniesRequest message, IMessageHandlerContext context)
		{
			log.Info("Received GetCompaniesRequest");

      var companies = await _companyRepository.GetAllCompaniesAsync();

      var response = new GetCompaniesResponse()
      {
        DataId = message.DataId,
        Companies = companies.ToList()
      };

      await context.Reply(response);
    }
	}
}