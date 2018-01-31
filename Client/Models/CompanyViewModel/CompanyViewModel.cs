namespace Client.Models.CompanyViewModel
{
	using System;
	using System.ComponentModel.DataAnnotations;

	public class CompanyViewModel
    {
	    public Guid Id { get; set; }

	    [Display(Name = "Created date")]
	    public string CreationTime { get; set; }
	    [Display(Name = "Name")]
	    public string Name { get; set; }

	    [Display(Name = "Address")]
	    public string Address { get; set; }
	}
}