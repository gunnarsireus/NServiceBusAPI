using System;
using System.Collections.Generic;
using Shared.Models;

namespace Client.DAL
{
    public interface ICarDataAccess
    {
        Car GetCar(Guid id);
        ICollection<Car> GetCars();
        ICollection<Company> GetCompanies();
        Company GetCompany(Guid id);
    }
}