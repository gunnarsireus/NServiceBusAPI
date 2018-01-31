namespace Shared.Commands
{
	using System;

// What does update mean anyway?
	public class UpdateCarOnlineStatus
	{
		public Guid Id { get; set; }
		public bool Online { get; set; }
	}
}
