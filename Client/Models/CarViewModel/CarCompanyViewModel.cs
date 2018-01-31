namespace Client.Models.CarViewModel
{
	using System;
	using Shared.Models;

	public class CarCompanyViewModel
	{
		public CarCompany CarCompany { get; set; }
		public Guid CarId => CarCompany.Id;
		public Guid CompanyId => CarCompany.CompanyId;
	}
}