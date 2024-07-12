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
using System.Linq;
using System.Runtime.ConstrainedExecution;
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

        [HttpGet("/car/index")]
        public async Task<IActionResult> Index(Guid? id)
        {
            if (!_signInManager.IsSignedIn(User))
                return RedirectToAction("Index", "Home");

            try
            {
                var getCarsResponse = await _messageSession.Request<GetCarsResponse>(new GetCarsRequest());
                var getCompaniesResponse = await _messageSession.Request<GetCompaniesResponse>(new GetCompaniesRequest());

                var selectedCompany = id == null
                    ? getCompaniesResponse.Companies.FirstOrDefault()
                    : getCompaniesResponse.Companies.SingleOrDefault(c => c.Id == id);

                var companyId = selectedCompany?.Id ?? Guid.NewGuid();
                getCarsResponse.Cars = getCarsResponse.Cars.Where(c => c.CompanyId == companyId).ToList();

                var selectList = getCompaniesResponse.Companies.Select(c => new SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString(),
                    Selected = c.Id == companyId
                }).ToList();

                var viewModel = new CarListViewModel(companyId)
                {
                    CompanySelectList = selectList,
                    Cars = getCarsResponse.Cars
                };

                ViewBag.CompanyId = companyId;
                ViewBag.CompanyName = selectedCompany?.Name;

                return View(viewModel);
            }
            catch (Exception ex)
            {
                // Handle exception (log, display error message, etc.)
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet("/car/details")]
        public async Task<IActionResult> Details(Guid id)
        {
            var getCarResponse = await _messageSession.Request<GetCarResponse>(new GetCarRequest(id));
            var getCompanyResponse = await _messageSession.Request<GetCompanyResponse>(new GetCompanyRequest(getCarResponse.Car.CompanyId));
            ViewBag.CompanyName = getCompanyResponse.Company.Name;
            return View(getCarResponse.Car);
        }

        [HttpGet("/car/create")]
        public async Task<IActionResult> Create(Guid id)
        {
            var car = new Car(id);
            var getCompanyResponse = await _messageSession.Request<GetCompanyResponse>(new GetCompanyRequest(id));
            ViewBag.CompanyName = getCompanyResponse.Company.Name;
            return View(car);
        }

        // POST: Car/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("/car/create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CompanyId,VIN,RegNr,Online")] Car car)
        {
            try
            {
                if (car == null)
                {
                    // Log or handle null car object
                    return RedirectToAction("Index", "Home");
                }

                if (!ModelState.IsValid)
                {
                    return View(car);
                }

                car.Id = Guid.NewGuid();

                await _messageSession.Request<CreateCarResponse>(new CreateCarRequest(car));

                return RedirectToAction("Index", new { id = car?.CompanyId });
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Home");
            }
        }


        [HttpGet("/car/edit")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var getCarResponse = await _messageSession.Request<GetCarResponse>(new GetCarRequest(id));
            getCarResponse.Car.Disabled = true; //Prevent updates of Online/Offline while editing
            var upadteCarResponse = await _messageSession.Request<UpdateCarResponse>(new UpdateCarRequest(getCarResponse.Car));
            var getCompanyResponse = await _messageSession.Request<GetCompanyResponse>(new GetCompanyRequest(upadteCarResponse.Car.CompanyId));

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

            var oldCarResponse = await _messageSession.Request<GetCarResponse>(new GetCarRequest(id));
            var oldCar = oldCarResponse.Car;
            oldCar.Online = car.Online;
            oldCar.Disabled = false; //Enable updates of Online/Offline when editing done

            await _messageSession.Request<UpdateCarResponse>(new UpdateCarRequest(oldCar));

            return RedirectToAction("Index", new { id = oldCar.CompanyId });
        }

        [HttpGet("/car/delete")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var getCarResponse = await _messageSession.Request<GetCarResponse>(new GetCarRequest(id));

            return View(getCarResponse.Car);
        }

        // POST: Car/Delete/5
        [HttpPost("/car/delete")]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var getCarResponse = await _messageSession.Request<GetCarResponse>(new GetCarRequest(id));
            await _messageSession.Request<DeleteCarResponse>(new DeleteCarRequest(id));

            return RedirectToAction("Index", new { id = getCarResponse.Car.CompanyId });
        }

        [HttpGet("/car/regnravailableasync")]
        public async Task<JsonResult> RegNrAvailableAsync(string regNr)
        {
            var getCarsResponse = await _messageSession.Request<GetCarsResponse>(new GetCarsRequest());
            bool isAvailable = getCarsResponse.Cars.All(c => c.RegNr != regNr);

            return Json(isAvailable);
        }

        [HttpGet("/car/vinavailableasync")]
        public async Task<JsonResult> VinAvailableAsync(string vin)
        {
            var getCarsResponse = await _messageSession.Request<GetCarsResponse>(new GetCarsRequest());
            bool isAvailable = getCarsResponse.Cars.All(c => c.VIN != vin);

            return Json(isAvailable);
        }
    }
}