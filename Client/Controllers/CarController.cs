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
using Shared.Models;
using Client.DAL;
using Shared.Requests;

namespace Client.Controllers
{

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
        public async Task<IActionResult> UpdateOnline([FromBody] Car car)
        {
            if (!ModelState.IsValid) return Json(new { success = false });
            var oldCar = await _dataAccess.GetCar(car.Id);
            oldCar.Online = car.Online;
            var message = new UpdateCarRequest(car);
            await _endpointInstance.Send(message).ConfigureAwait(false);
            return Json(new { success = true });
        }

        // GET: Car
        public async Task<IActionResult> Index(string id)
        {
            if (!_signInManager.IsSignedIn(User)) return RedirectToAction("Index", "Home");
            var cars = await _dataAccess.GetCars();
            var companies = await _dataAccess.GetCompanies();
            if (companies.Any() && id == null)
                id = companies[0].Id.ToString();

            cars[0].CompanyId = companies[0].Id;

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
                Selected = company.Id.ToString() == id
            }));

            var companyId = Guid.NewGuid();
            if (id != null)
            {
                companyId = new Guid(id);
                cars = cars.Where(o => o.CompanyId == companyId).ToList();
            }

            var carListViewModel = new CarListViewModel(companyId)
            {
                CompanySelectList = selectList,
                Cars = cars
            };

            ViewBag.CompanyId = id;
            return View(carListViewModel);
        }

        // GET: Car/Details/5
        public async Task<IActionResult> Details(Guid id)
        {
            var car = await _dataAccess.GetCar(id);
            var company = await _dataAccess.GetCompany(car.CompanyId);
            ViewBag.CompanyName = company.Name;
            return View(car);
        }

        // GET: Car/Create
        public async Task<IActionResult> Create(string id)
        {
            var companyId = new Guid(id);
            var car = new Car(companyId);
            ViewBag.CompanyName = await _dataAccess.GetCompany(companyId);
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
            var message = new CreateCarRequest(car);
            await _endpointInstance.Send(message).ConfigureAwait(false);
            return RedirectToAction("Index", new { id = car.CompanyId });
        }

        // GET: Car/Edit/5
        public async Task<IActionResult> Edit(Guid id)
        {
            var car = await _dataAccess.GetCar(id);
            car.Disabled = true; //Prevent updates of Online/Offline while editing
            var message = new UpdateCarRequest(car);
            await _endpointInstance.Send(message).ConfigureAwait(false);
            var company = await _dataAccess.GetCompany(car.CompanyId);
            ViewBag.CompanyName = company.Name;
            return View(car);
        }

        // POST: Car/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id, Online")] Car car)
        {
            if (!ModelState.IsValid) return View(car);
            var oldCar = await _dataAccess.GetCar(id);
            oldCar.Online = car.Online;
            oldCar.Disabled = false; //Enable updates of Online/Offline when editing done
            var message = new UpdateCarRequest(car);
            await _endpointInstance.Send(message).ConfigureAwait(false);

            return RedirectToAction("Index", new { id = oldCar.CompanyId });
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
            var car = await _dataAccess.GetCar(id);
            var message = new DeleteCarRequest(id);
            await _endpointInstance.Send(message).ConfigureAwait(false);
            return RedirectToAction("Index", new { id = car.CompanyId });
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