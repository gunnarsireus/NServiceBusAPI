using System;

namespace Shared.Models
{
	public class CarDisabledStatus
	{
		public Guid Id { get; set; }
		public bool Disabled { get; set; } //Used to block changes of Online/Offline status
	}
}
