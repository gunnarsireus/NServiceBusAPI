using NServiceBus;
using System;

namespace Shared.Response
{
	[Serializable]
	public class GetDbLocationResponse : IMessage
	{
		public GetDbLocationResponse()
		{
			DataId = Guid.NewGuid();
		}
		public Guid DataId { get; set; }
		public string DbLocation { get; set; }

		public static implicit operator string(GetDbLocationResponse v)
		{
			throw new NotImplementedException();
		}
	}
}

