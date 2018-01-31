using System;
using System.Linq;
using Shared.Models;

namespace Shared.DAL
{
	public static class ServerExtensions
	{
		public static void EnsureSeedData(this CarApiContext context)
		{
			if (!context.Cars.Any() || !context.Companies.Any())
			{
				var companyId = Guid.NewGuid();
				context.Companies.Add(new Company(companyId)
				{
					Name = "Charlies Gravel Transports Ltd.",
					Address = "Concrete Road 8, 111 11 Newcastle"
				});

				var carID = Guid.NewGuid();
				context.Cars.Add(new Car
				{
					Id = carID,
					VIN = "YS2R4X20005399401",
					RegNr = "ABC123"
				});
				context.CarCompany.Add(new CarCompany
				{
					Id = carID,
					CompanyId = companyId
				});
				context.CarDisabledStatus.Add(new CarDisabledStatus
				{
					Id = carID,
					Disabled = false
				});
				context.CarOnlineStatus.Add(new CarOnlineStatus
				{
					Id = carID,
					Online = true
				});

				carID = Guid.NewGuid();
				context.Cars.Add(new Car
				{
					Id = carID,
					VIN = "VLUR4X20009093588",
					RegNr = "DEF456"
				});
				context.CarCompany.Add(new CarCompany
				{
					Id = carID,
					CompanyId = companyId
				});
				context.CarDisabledStatus.Add(new CarDisabledStatus
				{
					Id = carID,
					Disabled = false
				});
				context.CarOnlineStatus.Add(new CarOnlineStatus
				{
					Id = carID,
					Online = true
				});

				carID = Guid.NewGuid();
				context.Cars.Add(new Car
				{
					Id = carID,
					VIN = "VLUR4X20009048066",
					RegNr = "GHI789"
				});
				context.CarCompany.Add(new CarCompany
				{
					Id = carID,
					CompanyId = companyId
				});
				context.CarDisabledStatus.Add(new CarDisabledStatus
				{
					Id = carID,
					Disabled = false
				});
				context.CarOnlineStatus.Add(new CarOnlineStatus
				{
					Id = carID,
					Online = true
				});

				// new set
				companyId = Guid.NewGuid();
				context.Companies.Add(new Company(companyId) { Name = "Jonnies Bulk Ltd.", Address = "Balk Road 12, 222 22 London" });

				carID = Guid.NewGuid();
				context.Cars.Add(new Car
				{
					Id = carID,
					VIN = "YS2R4X20005388011",
					RegNr = "JKL012"
				});
				context.CarCompany.Add(new CarCompany
				{
					Id = carID,
					CompanyId = companyId
				});
				context.CarDisabledStatus.Add(new CarDisabledStatus
				{
					Id = carID,
					Disabled = false
				});
				context.CarOnlineStatus.Add(new CarOnlineStatus
				{
					Id = carID,
					Online = true
				});

				carID = Guid.NewGuid();
				context.Cars.Add(new Car
				{
					Id = carID,
					VIN = "YS2R4X20005387949",
					RegNr = "MNO345"
				});
				context.CarCompany.Add(new CarCompany
				{
					Id = carID,
					CompanyId = companyId
				});
				context.CarDisabledStatus.Add(new CarDisabledStatus
				{
					Id = carID,
					Disabled = false
				});
				context.CarOnlineStatus.Add(new CarOnlineStatus
				{
					Id = carID,
					Online = true
				});


				companyId = Guid.NewGuid();
				context.Companies.Add(new Company(companyId) { Name = "Harolds Road Transports Ltd.", Address = "Budget Avenue 1, 333 33 Birmingham" });
				carID = Guid.NewGuid();
				context.Cars.Add(new Car
				{
					Id = carID,
					VIN = "YS2R4X20005387765",
					RegNr = "PQR678"
				});
				context.CarCompany.Add(new CarCompany
				{
					Id = carID,
					CompanyId = companyId
				});
				context.CarDisabledStatus.Add(new CarDisabledStatus
				{
					Id = carID,
					Disabled = false
				});
				context.CarOnlineStatus.Add(new CarOnlineStatus
				{
					Id = carID,
					Online = true
				});

				carID = Guid.NewGuid();
				context.Cars.Add(new Car
				{
					Id = carID,
					VIN = "YS2R4X20005387055",
					RegNr = "STU901"
				});
				context.CarCompany.Add(new CarCompany
				{
					Id = carID,
					CompanyId = companyId
				});
				context.CarDisabledStatus.Add(new CarDisabledStatus
				{
					Id = carID,
					Disabled = false
				});
				context.CarOnlineStatus.Add(new CarOnlineStatus
				{
					Id = carID,
					Online = true
				});
			}
			else
			{
				foreach (var car in context.CarDisabledStatus)
				{
					car.Disabled = false;
				}
			}
			context.SaveChanges();
		}
	}
}
