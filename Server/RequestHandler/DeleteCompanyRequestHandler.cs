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
	public class DeleteCompanyRequestHandler : IHandleMessages<DeleteCompanyRequest>
	{
    readonly ICompanyRepository _companyRepository;

    public DeleteCompanyRequestHandler(ICompanyRepository caompanyRepository)
    {
      _companyRepository = caompanyRepository;
    }

    static ILog log = LogManager.GetLogger<GetCompanyRequest>();

    public async Task Handle(DeleteCompanyRequest message, IMessageHandlerContext context)
    {
      log.Info("Received DeleteCompanyRequest.");

      await _companyRepository.RemoveCompanyAsync(message.CompanyId);

      var response = new DeleteCompanyResponse()
      {
        DataId = message.DataId,
      };

      await context.Reply(response);
    }
  }
}