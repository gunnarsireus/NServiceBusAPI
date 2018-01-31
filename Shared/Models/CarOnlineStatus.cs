using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Microsoft.AspNetCore.Mvc;

namespace Shared.Models
{
	public class CarOnlineStatus
	{
		public Guid Id { get; set; }
		public bool Online { get; set; }

	}
}
