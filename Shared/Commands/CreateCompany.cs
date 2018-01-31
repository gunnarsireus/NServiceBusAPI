namespace Shared.Commands
{
	using System;
	using System.Collections.Generic;

	public class CreateCompany
	{
		public Guid DataId { get; set; }
		public Guid Id { get; set; }
		public string CreationTime { get; set; }
		public string Name { get; set; }
		public string Address { get; set; }
		public ICollection<Guid> Cars { get; set; }
	}
}
