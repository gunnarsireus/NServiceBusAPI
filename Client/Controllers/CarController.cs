using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Client.Models;
using NServiceBus;
using Client.Models.CarViewModel;
using Microsoft.AspNetCore.Cors;

namespace Client.Controllers
{
	using Client.DAL;
	using Shared.Commands;

	public class CarController : Controller
	{
		readonly SignInManager<ApplicationUser> _signInManager;
		readonly IEndpointInstance _endpointInstance;
		readonly CarDataAccess _dataAccess;

		public CarController(SignInManager<ApplicationUser> signInManager, IEndpointInstance endpointInstance, CarApiContext carApiContext)
		{
			_signInManager = signInManager;
			_endpointInstance = endpointInstance;
			_dataAccess = new CarDataAccess(carApiContext);

		}

		[HttpGet]
		[EnableCors("AllowAllOrigins")]
		public async Task<IActionResult> GetAllCars()
		{
			return Json(await _dataAccess.GetCars());
		}

		[HttpPost]
		public async Task<IActionResult> UpdateOnline([FromBody] CarViewModel car)
		{
			if (!ModelState.IsValid) return Json(new { success = false });
			var message = new UpdateCarOnlineStatus
			{
				Online = car.CarOnlineStatus.Online,
				Id = car.Car.Id
			};
			// TODO: map object to massege

			await _endpointInstance.Send(message).ConfigureAwait(false);

			// here we can get the latest data?
			return Json(new { success = true });
		}

		// GET: Car
		public async Task<IActionResult> Index(string compId)
		{
			if (!_signInManager.IsSignedIn(User)) return RedirectToAction("Index", "Home");
			var cars = await _dataAccess.GetCars();
			var carCompanies = await _dataAccess.GetCarCompany();
			var companies = await _dataAccess.GetCompanies();
			if (companies.Any() && compId == null)
				compId = companies[0].Id.ToString();

			cars.Cars.CarCompany.CompanyId = companies[0].Id;

			var selectList = new List<SelectListItem>
			{
				new SelectListItem
				{
					Text = "Choose company",
					Value = ""
				}
			};

			selectList.AddRange(companies.Select(company => new SelectListItem
			{
				Text = company.Name,
				Value = company.Id.ToString(),
				Selected = company.Id.ToString() == compId
			}));

			var companyId = Guid.NewGuid();
			if (compId != null)
			{
				companyId = new Guid(compId);
				cars = cars.Where(o => o.CarCompany.CompanyId == companyId).ToList();
			}

			var carListViewModel = new CarListViewModel(companyId)
			{
				CompanySelectList = selectList,
				Cars = cars
			};

			ViewBag.CompanyId = compId;
			return View(carListViewModel);
		}

		// GET: Car/Details/5
		public async Task<IActionResult> Details(Guid id)
		{
			var car = await _dataAccess.GetCar(id);
			var company = await _dataAccess.GetCompanyByCompanyId(car.CarCompany.CompanyId);
			ViewBag.CompanyName = company.Name;
			return View(car);
		}

		// GET: Car/Create
		public async Task<IActionResult> Create(string id)
		{
			var companyId = new Guid(id);
			var car = new CarViewModel
			{
				CarCompany =
				{
					CompanyId = companyId
				}
			};

			// go to an web API
			ViewBag.CompanyName = await _dataAccess.GetCompanyByCompanyId(companyId);
			return View(car);
		}

		// POST: Car/Create
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(
			[Bind("CompanyId,VIN,RegNr,Online")] Car car)
		{
			if (!ModelState.IsValid) return View(car);
			car.Id = Guid.NewGuid();

			var message = new CreateCar();
			// TODO: map object to massege

			await _endpointInstance.Send(message).ConfigureAwait(false);
			return RedirectToAction("Index", new { id = car.CompanyId });
		}

		// GET: Car/Edit/5
		public async Task<IActionResult> Edit(Guid id)
		{
			var carCompany = await _dataAccess.GetCarCompany(id);
			var company = await _dataAccess.GetCompanyByCompanyId(carCompany.CompanyId);
			ViewBag.CompanyName = company.Name;
			return View(carCompany);
		}

		// POST: Car/Edit/5
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(Guid id, [Bind("Id, Online")] Car car)
		{
			if (!ModelState.IsValid) return View(car);
			//var oldCar = await _dataAccess.GetCar(compId);
			//oldCar.Online = car.Online;
			//oldCar.Disabled = false; //Enable updates of Online/Offline when editing done

			var message = new UpdateCar
			{
				Online = car.Online,
				Disabled = false //?? Enable updates of Online/Offline when editing done
			};

			await _endpointInstance.Send(message).ConfigureAwait(false);

			return RedirectToAction("Index", new { id = car.CompanyId });
		}

		// GET: Car/Delete/5
		public async Task<IActionResult> Delete(Guid id)
		{
			return View(await _dataAccess.GetCar(id));
		}

		// POST: Car/Delete/5
		[HttpPost]
		[ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(Guid id)
		{
			var message = new DeleteCar() { CarId = id };
			// TODO: pass in the company compId from the ui
			var companyId = Guid.NewGuid();

			await _endpointInstance.Send(message).ConfigureAwait(false);

			return RedirectToAction("Index", new { id = companyId });
		}

		public async Task<bool> RegNrAvailableAsync(string regNr)
		{
			var cars = await _dataAccess.GetCars();
			return cars.All(c => c.RegNr != regNr);
		}

		public async Task<bool> VinAvailableAsync(string vin)
		{
			var cars = await _dataAccess.GetCars();
			return cars.All(c => c.VIN != vin);
		}
	}
}