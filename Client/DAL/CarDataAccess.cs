using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Client.DAL
{
	using System.Linq;
	using Client.Models.CarViewModel;
	using Client.Models.CompanyViewModel;

	public class CarDataAccess
	{
		readonly CarApiContext _carApiContext;

		public CarDataAccess(CarApiContext carApiContext)
		{
			_carApiContext = carApiContext;
		}

		public async Task<CarListViewModel> GetCars()
		{
			var list = new CarListViewModel();
			var cars = await _carApiContext.Cars.ToListAsync();
			
			foreach (var car in cars)
			{
				var c = new CarViewModel
				{
					Car =
					{
						Id = car.Id,
						CreationTime = car.CreationTime,
						RegNr = car.RegNr,
						VIN = car.VIN
					}
				};
			}
			return list;
		}

		public async Task<CarViewModel> GetCar(Guid id)
		{
			var car = await _carApiContext.Cars.SingleOrDefaultAsync(o => o.Id == id);
			var carViewModel = new CarViewModel
			{
				Car =
				{
					Id = car.Id,
					CreationTime = car.CreationTime,
					RegNr = car.RegNr,
					VIN = car.VIN
				},
			};
			return carViewModel;
		}

		public async Task<CompanyViewModel> GetCarCompany(Guid carId)
		{
			var carCompany = await _carApiContext.CarCompany.SingleOrDefaultAsync(o => o.Id == carId);
			var company = await GetCompanyByCompanyId(carCompany.CompanyId);
			var companyViewModel = new CompanyViewModel
			{
				Id = company.Id,
				CreationTime = company.CreationTime,
				Address = company.Address,
				Name = company.Name
			};

			return companyViewModel;
		}

		public async Task<List<CompanyViewModel>> GetCarCompanies()
		{
			var carCompanies = await _carApiContext.CarCompany.ToListAsync();
			var allCompanies = await _carApiContext.Companies.ToListAsync();
			var list = new List<CompanyViewModel>();
			foreach (var companyId in carCompanies)
			{
				var company = allCompanies.FirstOrDefault(c => c.Id == companyId.CompanyId);
				if (company != null)
				{
					var companyViewModel = new CompanyViewModel
					{
						Id = company.Id,
						CreationTime = company.CreationTime,
						Address = company.Address,
						Name = company.Name
					};
					list.Add(companyViewModel);
				}
			}

			return list;
		}

		public async Task<List<CompanyViewModel>> GetCompanies()
		{
			var list = new List<CompanyViewModel>();
			var companies = await _carApiContext.Companies.ToListAsync();
			foreach (var company in companies)
			{
				var companyViewModel = new CompanyViewModel
				{
					Id = company.Id,
					CreationTime = company.CreationTime,
					Address = company.Address,
					Name = company.Name
				};
				list.Add(companyViewModel);
			}
			return list;
		}

		public async Task<CompanyViewModel> GetCompanyByCompanyId(Guid companyId)
		{
			var company = await _carApiContext.Companies.SingleOrDefaultAsync(o => o.Id == companyId);
			var companyViewModel = new CompanyViewModel
			{
				Id = company.Id,
				CreationTime = company.CreationTime,
				Address = company.Address,
				Name = company.Name
			};

			return companyViewModel;
		}
	}
}