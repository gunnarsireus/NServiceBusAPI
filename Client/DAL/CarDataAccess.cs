using System;
using System.Collections.Generic;
using System.Linq;
using Shared.Models;

namespace Client.DAL
{

	public class CarDataAccess : ICarDataAccess
	{
		readonly CarApiContext _carApiContext;

		public CarDataAccess(CarApiContext carApiContext)
		{
			_carApiContext = carApiContext;
		}

		public ICollection<Car> GetCars()
		{
			return _carApiContext.Cars.ToList();
		}

		public Car GetCar(Guid id)
		{
			return _carApiContext.Cars.SingleOrDefault(o => o.Id == id);
		}

		public void AddCar(Car car)
		{
			_carApiContext.Cars.Add(car);
			_carApiContext.SaveChanges();
		}

		public void DeleteCar(Guid id)
		{
			var Car = GetCar(id);
			_carApiContext.Cars.Remove(Car);
			_carApiContext.SaveChanges();
		}

		public void UpdateCar(Car car)
		{
			_carApiContext.Cars.Update(car);
			_carApiContext.SaveChanges();
		}

		public ICollection<Company> GetCompanies()
		{
			return _carApiContext.Companies.ToList();
		}

		public Company GetCompany(Guid id)
		{
			return _carApiContext.Companies.SingleOrDefault(o => o.Id == id);
		}

		public void AddCompany(Company company)
		{
			_carApiContext.Companies.Add(company);
			_carApiContext.SaveChanges();
		}

		public void DeleteCompany(Guid id)
		{
			var company = GetCompany(id);
			_carApiContext.Companies.Remove(company);
			_carApiContext.SaveChanges();
		}

		public void UpdateCompany(Company company)
		{
			_carApiContext.Companies.Update(company);
			_carApiContext.SaveChanges();
		}
	}
}