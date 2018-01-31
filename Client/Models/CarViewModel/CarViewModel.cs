namespace Client.Models.CarViewModel
{
	using System;
	using System.ComponentModel.DataAnnotations;
	using Microsoft.AspNetCore.Mvc;
	using Shared.Models;

	public class CarViewModel
	{
		public Car Car { get; set; }
		public CarCompany CarCompany { get; set; }
		public CarOnlineStatus CarOnlineStatus { get; set; }
		public CarDisabledStatus CarDisabledStatus { get; set; }
		[Display(Name = "Online (X) or Offline ()?")]
		public string OnlineOrOffline => (CarOnlineStatus.Online) ? "Online" : "Offline";

		public Guid Id => Car.Id;

		[Display(Name = "Created date")]
		public string CreationTime => Car.CreationTime;

		[Display(Name = "VIN (VehicleID)")]
		[RegularExpression(@"^[A-Z0-9]{6}\d{11}$", ErrorMessage = "{0} denoted as X1Y2Z300001239876")]
		[Remote("VinAvailableAsync", "Car", ErrorMessage = "VIN already taken")]
		public string VIN => Car.VIN;

		[Display(Name = "Reg. Nbr.")]
		[RegularExpression(@"^[A-Z]{3}\d{3}$", ErrorMessage = "{0} denoted as XYZ123")]
		[Remote("RegNrAvailableAsync", "Car", ErrorMessage = "Registration number taken")]
		public string RegNr => Car.RegNr;


	}
}