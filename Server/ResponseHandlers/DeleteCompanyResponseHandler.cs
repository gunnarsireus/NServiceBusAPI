using NServiceBus;
using NServiceBus.Logging;
using Server.Data;
using Shared.Responses;
using System.Threading.Tasks;

namespace Server.ResponseHandlers
{
  public class DeleteCompanyResponseHandler : IHandleMessages<DeleteCompanyResponse>
	{
    private readonly ICompanyRepository _companyRepository;

    public DeleteCompanyResponseHandler(ICompanyRepository caompanyRepository)
    {
      _companyRepository = caompanyRepository;
    }

    static ILog log = LogManager.GetLogger<GetCompanyResponse>();

    public async Task Handle(DeleteCompanyResponse message, IMessageHandlerContext context)
    {
      log.Info("Received DeleteCompanyResponse.");

      //await _companyRepository.RemoveCompanyAsync(message.CompanyId);

      //var response = new DeleteCompanyResponse()
      //{
      //  DataId = message.DataId,
      //};

      //await context.Reply(response);
    }
  }
}