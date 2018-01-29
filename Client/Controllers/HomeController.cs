using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Client.Models.HomeViewModel;
using Client.Models;
using NServiceBus;
using Shared.Models;
using Client.DAL;
using Shared.Requests;

namespace Client.Controllers
{
	public class HomeController : Controller
	{
		readonly IEndpointInstance _endpointInstance;
        readonly CarDataAccess _dataAccess;

        public HomeController(IEndpointInstance endPointEndpointInstance)
		{
			_endpointInstance = endPointEndpointInstance;
            _dataAccess = new CarDataAccess();
        }

		public async Task<IActionResult> Index()
		{
			List<Company> companies;
			try
			{
				companies = _dataAccess.GetCompanies().ToList();
			}
			catch (Exception e)
			{
				TempData["CustomError"] = "Ingen kontakt med servern! CarAPI måste startas innan Client kan köras!";
				return View("Index", new HomeViewModel(Guid.NewGuid()) { Companies = new List<Company>() });
			}

			var allCars = _dataAccess.GetCars().ToList();
			foreach (var car in allCars)
			{
				car.Disabled = false; //Enable updates of Online/Offline
                var message = new UpdateCarRequest(car);
                await _endpointInstance.Send(message).ConfigureAwait(false);
            }

			foreach (var company in companies)
			{
				var companyCars = allCars.Where(o => o.CompanyId == company.Id).ToList();
				company.Cars = companyCars;
			}
			var homeViewModel = new HomeViewModel(Guid.NewGuid()) { Companies = companies };
			return View("Index", homeViewModel);
		}

		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}