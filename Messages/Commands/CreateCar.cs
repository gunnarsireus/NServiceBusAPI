namespace Messages.Commands
{
	using System;

	public class CreateCar
	{
		public Guid DataId { get; set; }
		public Guid Id { get; set; }
		public Guid CompanyId { get; set; }
		public string CreationTime { get; set; }
		public string VIN { get; set; }
		public string RegNr { get; set; }
		public bool Online { get; set; }
		public bool Disabled { get; set; } //Used to block changes of Online/Offline status
	}
}
