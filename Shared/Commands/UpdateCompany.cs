namespace Shared.Commands
{
	using System;
	using System.Collections.Generic;

	// What does update mean anyway?
	public class UpdateCompany
	{
		public Guid DataId { get; set; }
		public Guid Id { get; set; }
		public string CreationTime { get; set; }
		public string Name { get; set; }
		public string Address { get; set; }
		public ICollection<Guid> Cars { get; set; }
	}
}
