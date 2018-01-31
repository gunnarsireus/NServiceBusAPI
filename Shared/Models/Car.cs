using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Shared.Models
{
	public class Car
	{
		public Guid Id { get; set; }

		[Display(Name = "Created date")]
		public string CreationTime { get; set; }

		[Display(Name = "VIN (VehicleID)")]
		[RegularExpression(@"^[A-Z0-9]{6}\d{11}$", ErrorMessage = "{0} denoted as X1Y2Z300001239876")]
		[Remote("VinAvailableAsync", "Car", ErrorMessage = "VIN already taken")]
		public string VIN { get; set; }

		[Display(Name = "Reg. Nbr.")]
		[RegularExpression(@"^[A-Z]{3}\d{3}$", ErrorMessage = "{0} denoted as XYZ123")]
		[Remote("RegNrAvailableAsync", "Car", ErrorMessage = "Registration number taken")]
		public string RegNr { get; set; }
	}
}
