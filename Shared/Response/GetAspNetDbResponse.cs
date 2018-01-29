using NServiceBus;
using System;

namespace Shared.Response
{
	[Serializable]
	public class GetDbLocationsResponse : IMessage
	{
		public GetDbLocationsResponse()
		{
			DataId = Guid.NewGuid();
		}
		public Guid DataId { get; set; }
		public string DbLocation { get; set; }

		public static implicit operator string(GetDbLocationsResponse v)
		{
			throw new NotImplementedException();
		}
	}
}

