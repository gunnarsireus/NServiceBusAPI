using Client.Models;
using Client.Models.CarViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NServiceBus;
using Shared.Models;
using Shared.Requests;
using Shared.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Client.Controllers
{

    [Route("/car")]
    public class CarController : Controller
    {
        readonly SignInManager<ApplicationUser> _signInManager;
        readonly IMessageSession _messageSession;

        public CarController(SignInManager<ApplicationUser> signInManager, IMessageSession messageSession)
        {
            _signInManager = signInManager;
            _messageSession = messageSession;
        }

        [HttpGet("/car/getallcars")]
        public async Task<IActionResult> GetAllCars()
        {
            var getCarsResponse = await Utils.Utils.GetCarsResponseAsync(_messageSession);
            return Json(getCarsResponse.Cars);
        }

        [HttpPost("/car/updateonline")]
        public async Task<IActionResult> UpdateOnline([FromBody] Car car)
        {
            if (!ModelState.IsValid) return Json(new { success = false });
            var getCarResponse = await Utils.Utils.GetCarResponseAsync(car.Id, _messageSession);
            var oldCar = getCarResponse.Car;
            oldCar.Online = car.Online;
            await Utils.Utils.UpdateCarResponseAsync(oldCar, _messageSession);
            return Json(new { success = true });
        }

        [HttpGet("/car/index")]
        public async Task<IActionResult> Index(string id)
        {
            if (!_signInManager.IsSignedIn(User)) return RedirectToAction("Index", "Home");
            var getCarsResponse = await Utils.Utils.GetCarsResponseAsync(_messageSession);

            var getCompaniesResponse = await Utils.Utils.GetCompaniesResponseAsync(_messageSession);

            if (getCompaniesResponse.Companies.Any() && id == null)
                id = getCompaniesResponse.Companies[0].Id.ToString();

            getCarsResponse.Cars[0].CompanyId = getCompaniesResponse.Companies[0].Id;

            var selectList = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Choose company",
                    Value = ""
                }
            };

            selectList.AddRange(getCompaniesResponse.Companies.Select(company => new SelectListItem
            {
                Text = company.Name,
                Value = company.Id.ToString(),
                Selected = company.Id.ToString() == id
            }));

            var companyId = Guid.NewGuid();
            if (id != null)
            {
                companyId = new Guid(id);
                getCarsResponse.Cars = getCarsResponse.Cars.Where(o => o.CompanyId == companyId).ToList();
            }

            var carListViewModel = new CarListViewModel(companyId)
            {
                CompanySelectList = selectList,
                Cars = getCarsResponse.Cars
            };

            ViewBag.CompanyId = id;
            return View(carListViewModel);
        }


        [HttpGet("/car/details")]
        public async Task<IActionResult> Details(Guid id)
        {
            var getCarResponse = await Utils.Utils.GetCarResponseAsync(id, _messageSession);
            var getCompanyResponse = await Utils.Utils.GetCompanyResponseAsync(getCarResponse.Car.CompanyId, _messageSession);
            ViewBag.CompanyName = getCompanyResponse.Company.Name;
            return View(getCarResponse.Car);
        }

        [HttpGet("/car/create")]
        public async Task<IActionResult> Create(string id)
        {
            var companyId = new Guid(id);
            var car = new Car(companyId);
            var getCompanyResponse = await Utils.Utils.GetCompanyResponseAsync(companyId, _messageSession);
            ViewBag.CompanyName = getCompanyResponse.Company.Name;
            return View(car);
        }

        // POST: Car/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("/car/create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
                [Bind("CompanyId,VIN,RegNr,Online")] Car car)
        {
            if (!ModelState.IsValid) return View(car);

            car.Id = Guid.NewGuid();
            // var createCarResponse = await Utils.Utils.CreateCarResponseAsync(car, _messageSession);

            await _messageSession
                .Request<CreateCarResponse>(new CreateCarRequest(car));

            return RedirectToAction("Index", new { id = car.CompanyId });
        }

        [HttpGet("/car/edit")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var getCarResponse = await Utils.Utils.GetCarResponseAsync(id, _messageSession);
            getCarResponse.Car.Disabled = true; //Prevent updates of Online/Offline while editing
            var updateCarResponse = Utils.Utils.UpdateCarResponseAsync(getCarResponse.Car, _messageSession);
            var getCompanyResponse = await Utils.Utils.GetCompanyResponseAsync(getCarResponse.Car.CompanyId, _messageSession);

            ViewBag.CompanyName = getCompanyResponse.Company.Name;

            return View(getCarResponse.Car);
        }

        // POST: Car/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("/car/edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id, Online")] Car car)
        {
            if (!ModelState.IsValid) return View(car);

            var oldCarResponse = await Utils.Utils.GetCarResponseAsync(id, _messageSession);
            var oldCar = oldCarResponse.Car;
            oldCar.Online = car.Online;
            oldCar.Disabled = false; //Enable updates of Online/Offline when editing done

            var updateCarResponse = Utils.Utils.UpdateCarResponseAsync(oldCar, _messageSession);

            return RedirectToAction("Index", new { id = oldCar.CompanyId });
        }

        [HttpGet("/car/delete")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var getCarResponse = await Utils.Utils.GetCarResponseAsync(id, _messageSession);

            return View(getCarResponse.Car);
        }

        // POST: Car/Delete/5
        [HttpPost("/car/delete")]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var getCarResponse = await Utils.Utils.GetCarResponseAsync(id, _messageSession);
            await Utils.Utils.DeleteCarResponseAsync(id, _messageSession);

            return RedirectToAction("Index", new { id = getCarResponse.Car.CompanyId });
        }

        [HttpGet("/car/regnravailableasync")]
        public async Task<JsonResult> RegNrAvailableAsync(string regNr)
        {
            var getCarsResponse = await Utils.Utils.GetCarsResponseAsync(_messageSession);
            bool isAvailable = getCarsResponse.Cars.All(c => c.RegNr != regNr);

            return Json(isAvailable);
        }

        [HttpGet("/car/vinavailableasync")]
        public async Task<JsonResult> VinAvailableAsync(string vin)
        {
            var getCarsResponse = await Utils.Utils.GetCarsResponseAsync(_messageSession);
            bool isAvailable = getCarsResponse.Cars.All(c => c.VIN != vin);

            return Json(isAvailable);
        }
    }
}