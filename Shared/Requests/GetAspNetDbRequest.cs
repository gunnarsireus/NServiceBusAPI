using System;
using NServiceBus;

namespace Shared.Requests
{
	[Serializable]
	public class GetDbLocationRequest : IMessage
	{
		public GetDbLocationRequest()
		{
			DataId = Guid.NewGuid();
		}
		public Guid DataId { get; set; }
	}
}
