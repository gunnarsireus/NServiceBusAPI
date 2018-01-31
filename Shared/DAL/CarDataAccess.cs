using System;
using System.Collections.Generic;
using Shared.Models;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Shared.DAL
{
	public class CarDataAccess
	{
		readonly CarApiContext _carApiContext;

		public CarDataAccess(CarApiContext carApiContext)
		{
			_carApiContext = carApiContext;
		}

		public async Task<List<Car>> GetCars()
		{
			return await _carApiContext.Cars.ToListAsync();
		}

		public async Task<Car> GetCar(Guid id)
		{
			return await _carApiContext.Cars.SingleOrDefaultAsync(o => o.Id == id);
		}

		public async Task<CarCompany> GetCarCompany(Guid id)
		{
			return await _carApiContext.CarCompany.SingleOrDefaultAsync(o => o.Id == id);
		}

		public async Task AddCar(Car car)
		{
			_carApiContext.Cars.Add(car);
			await _carApiContext.SaveChangesAsync();
		}

		public async Task DeleteCar(Guid id)
		{
			var car = await GetCar(id);
			_carApiContext.Cars.Remove(car);
			await _carApiContext.SaveChangesAsync();
		}

		public async Task UpdateCar(Car car)
		{
		    _carApiContext.Cars.Update(car);
			await _carApiContext.SaveChangesAsync();
		}

		public async Task<List<Company>> GetCompanies()
		{
			return await _carApiContext.Companies.ToListAsync();
		}

		public async Task<Company> GetCompany(Guid id)
		{
			return await _carApiContext.Companies.SingleOrDefaultAsync(o => o.Id == id);
		}

		public async Task AddCompany(Company company)
		{
			_carApiContext.Companies.Add(company);
			await _carApiContext.SaveChangesAsync();
		}

		public async Task DeleteCompany(Guid id)
		{
			var company = await GetCompany(id);
			_carApiContext.Companies.Remove(company);
			await _carApiContext.SaveChangesAsync();
		}

		public async Task UpdateCompany(Company company)
		{
			_carApiContext.Companies.Update(company);
			await _carApiContext.SaveChangesAsync();
		}
	}
}