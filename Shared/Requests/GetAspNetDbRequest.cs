using System;
using NServiceBus;

namespace Shared.Requests
{
	[Serializable]
	public class GetDbLocationsRequest : IMessage
	{
		public GetDbLocationsRequest()
		{
			DataId = Guid.NewGuid();
		}
		public Guid DataId { get; set; }
	}
}
