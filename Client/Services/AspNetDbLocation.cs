using System.Threading.Tasks;
using NServiceBus;
using Shared.Requests;
using Shared.Response;

namespace Client.Services
{
	// This class is used by the application to get the location of the AspNet.db used for authntication of users.
	public class dBlocations : IAspNetDbLocation
    {
	    public async Task<GetDbLocationsResponse> GetDbLocationsAsync(IEndpointInstance endpointInstance)
	    {
		    var message = new GetDbLocationsRequest();
		    var sendOptions = new SendOptions();
		    sendOptions.SetDestination("NServiceBusCore.Server");
		    var responseTask = await endpointInstance
			    .Request<GetDbLocationsResponse>(message, sendOptions);
		    return responseTask;
		}
	}
}
