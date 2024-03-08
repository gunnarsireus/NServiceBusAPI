using NServiceBus;
using NServiceBus.Logging;
using Server.Data;
using Shared.Responses;
using System.Threading.Tasks;

namespace Server.ResponseHandlers
{
  public class GetCompaniesResponseHandler : IHandleMessages<GetCompaniesResponse>
	{
    private readonly ICompanyRepository _companyRepository;

    public GetCompaniesResponseHandler(ICompanyRepository caompanyRepository)
    {
      _companyRepository = caompanyRepository;
    }

    static ILog log = LogManager.GetLogger<GetCompaniesResponse>();

		public async Task Handle(GetCompaniesResponse message, IMessageHandlerContext context)
		{
			log.Info("Received GetCompaniesResponse");

      var companies = await _companyRepository.GetAllCompaniesAsync();

      //var response = new GetCompaniesResponse()
      //{
      //  DataId = message.DataId,
      //  Companies = companies.ToList()
      //};

      //await context.Reply(response);
    }
	}
}