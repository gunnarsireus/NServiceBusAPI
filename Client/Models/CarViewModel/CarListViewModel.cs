using System.Collections.Generic;
using System;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Client.Models.CarViewModel
{
	public class CarListViewModel : CarViewModel
    {
	    public CarListViewModel()
	    {
		    
	    }
		public CarListViewModel(Guid companyId)
		{
			CompanyId = companyId;
		}
	    public Guid CompanyId { get; set; }
		public List<SelectListItem> CompanySelectList { get; set; }
		public List<CarViewModel> Cars { get; set; }
	}
}