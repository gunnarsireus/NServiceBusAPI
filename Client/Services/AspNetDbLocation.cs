using System.Threading.Tasks;
using NServiceBus;
using Shared.Requests;
using Shared.Response;

namespace Client.Services
{
	// This class is used by the application to get the location of the AspNet.db used for authntication of users.
	public class dBlocations
    {
	    public async Task<GetDbLocationResponse> GetDbLocationAsync(IEndpointInstance endpointInstance)
	    {
		    var message = new GetDbLocationRequest();
		    var sendOptions = new SendOptions();
		    sendOptions.SetDestination("NServiceBusCore.Server");
		    var responseTask = await endpointInstance
			    .Request<GetDbLocationResponse>(message, sendOptions);
		    return responseTask;
		}
	}
}
