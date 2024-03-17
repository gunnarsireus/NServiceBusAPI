using Client.Models;
using Client.Models.HomeViewModel;
using Microsoft.AspNetCore.Mvc;
using NServiceBus;
using Shared.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Client.Controllers
{
    public class HomeController : Controller
    {
        private readonly IMessageSession _messageSession;

        public HomeController(IMessageSession messageSession)
        {
            _messageSession = messageSession ?? throw new ArgumentNullException(nameof(messageSession));
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var companiesResponse = await Utils.Utils.GetCompaniesResponseAsync(_messageSession);
                var companies = companiesResponse.Companies;

                var carsResponse = await Utils.Utils.GetCarsResponseAsync(_messageSession);
                var allCars = carsResponse.Cars.ToList();

                // Batch update cars
                var updateTasks = allCars.Select(car => Utils.Utils.UpdateCarResponseAsync(car, _messageSession));
                await Task.WhenAll(updateTasks);

                foreach (var company in companies)
                {
                    var companyCars = allCars.Where(o => o.CompanyId == company.Id).ToList();
                    company.Cars = companyCars;
                }

                var homeViewModel = new HomeViewModel(Guid.NewGuid()) { Companies = companies };
                return View("Index", homeViewModel);
            }
            catch (Exception e)
            {
                TempData["CustomError"] = "Ingen kontakt med servern! CarAPI måste startas innan Client kan köras!";
                return View("Index", new HomeViewModel(Guid.NewGuid()) { Companies = new List<Company>() });
            }
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

}