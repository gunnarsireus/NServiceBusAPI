using System.Threading.Tasks;
using Shared.Response;

namespace Client.Services
{
	using NServiceBus;

	public interface IAspNetDbLocation
	{
		Task<GetDbLocationsResponse> GetDbLocationsAsync(IEndpointInstance endpointInstance);

	}
}
