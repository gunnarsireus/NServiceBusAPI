using NServiceBus;
using NServiceBus.Logging;
using Server.Data;
using Shared.Responses;
using System.Threading.Tasks;

namespace Server.ResponseHandlers
{
  public class CreateCompanyResponseHandler : IHandleMessages<CreateCompanyResponse>
	{
    private readonly ICompanyRepository _companyRepository;

    public CreateCompanyResponseHandler(ICompanyRepository companyRepository)
    {
      _companyRepository = companyRepository;
    }

    static ILog log = LogManager.GetLogger<CreateCompanyResponse>();

    public Task Handle(CreateCompanyResponse message, IMessageHandlerContext context)
    {
      log.Info("Received CreateCompanyResponse.");
      return Task.CompletedTask;
    }
  }
}