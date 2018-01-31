namespace Messages.Commands
{
	using System;

	public class DeleteCompany
	{
		public Guid DataId { get; set; }
		public Guid CompanyId { get; set; }
	}
}
