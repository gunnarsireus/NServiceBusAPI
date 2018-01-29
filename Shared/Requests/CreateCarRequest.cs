using System;
using NServiceBus;
using Shared.Models;

namespace Shared.Requests
{

	[Serializable]
	public class CreateCarRequest : ICommand
	{
		public CreateCarRequest(Car car)
		{
			DataId = Guid.NewGuid();
			Car = car;
		}
		public Guid DataId { get; set; }
		public Car Car { get; set; }
	}
}
