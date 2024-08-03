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
using System.Threading.Tasks;

namespace Client.Controllers
{
    [Route("car")]
    public class CarController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IMessageSession _messageSession;

        public CarController(SignInManager<ApplicationUser> signInManager, IMessageSession messageSession)
        {
            _signInManager = signInManager;
            _messageSession = messageSession;
        }

        [HttpGet("index")]
        public async Task<IActionResult> Index(Guid? id)
        {
            if (!_signInManager.IsSignedIn(User))
                return RedirectToAction("Index", "Home");

            try
            {
                var getCarsResponse = await _messageSession.Request<GetCarsResponse>(new GetCarsRequest()).ConfigureAwait(false);
                var getCompaniesResponse = await _messageSession.Request<GetCompaniesResponse>(new GetCompaniesRequest()).ConfigureAwait(false);

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
                // Example: _logger.LogError(ex, "Error fetching car data");
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet("details")]
        public async Task<IActionResult> Details(Guid id)
        {
            try
            {
                var getCarResponse = await _messageSession.Request<GetCarResponse>(new GetCarRequest(id)).ConfigureAwait(false);
                var getCompanyResponse = await _messageSession.Request<GetCompanyResponse>(new GetCompanyRequest(getCarResponse.Car.CompanyId)).ConfigureAwait(false);
                ViewBag.CompanyName = getCompanyResponse.Company.Name;
                return View(getCarResponse.Car);
            }
            catch (Exception ex)
            {
                // Handle exception (log, display error message, etc.)
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet("create")]
        public async Task<IActionResult> Create(Guid id)
        {
            try
            {
                var getCompanyResponse = await _messageSession.Request<GetCompanyResponse>(new GetCompanyRequest(id)).ConfigureAwait(false);
                ViewBag.CompanyName = getCompanyResponse.Company.Name;
                return View(new Car(id));
            }
            catch (Exception ex)
            {
                // Handle exception (log, display error message, etc.)
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CompanyId,VIN,RegNr,Online")] Car car)
        {
            if (car == null) return RedirectToAction("Index", "Home");

            if (!ModelState.IsValid) return View(car);

            try
            {
                await _messageSession.Request<CreateCarResponse>(new CreateCarRequest(car)).ConfigureAwait(false);
                return RedirectToAction("Index", new { id = car.CompanyId });
            }
            catch (Exception ex)
            {
                // Handle exception (log, display error message, etc.)
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet("edit")]
        public async Task<IActionResult> Edit(Guid id)
        {
            try
            {
                var getCarResponse = await _messageSession.Request<GetCarResponse>(new GetCarRequest(id)).ConfigureAwait(false);
                getCarResponse.Car.Disabled = true; //Prevent updates of Online/Offline while editing
                await _messageSession.Request<UpdateCarResponse>(new UpdateCarRequest(getCarResponse.Car)).ConfigureAwait(false);
                var getCompanyResponse = await _messageSession.Request<GetCompanyResponse>(new GetCompanyRequest(getCarResponse.Car.CompanyId)).ConfigureAwait(false);

                ViewBag.CompanyName = getCompanyResponse.Company.Name;
                return View(getCarResponse.Car);
            }
            catch (Exception ex)
            {
                // Handle exception (log, display error message, etc.)
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost("edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id, Online")] Car car)
        {
            if (!ModelState.IsValid) return View(car);

            try
            {
                var oldCarResponse = await _messageSession.Request<GetCarResponse>(new GetCarRequest(id)).ConfigureAwait(false);
                var oldCar = oldCarResponse.Car;
                oldCar.Online = car.Online;
                oldCar.Disabled = false; //Enable updates of Online/Offline when editing done

                await _messageSession.Request<UpdateCarResponse>(new UpdateCarRequest(oldCar)).ConfigureAwait(false);
                return RedirectToAction("Index", new { id = oldCar.CompanyId });
            }
            catch (Exception ex)
            {
                // Handle exception (log, display error message, etc.)
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet("delete")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var getCarResponse = await _messageSession.Request<GetCarResponse>(new GetCarRequest(id)).ConfigureAwait(false);
                return View(getCarResponse.Car);
            }
            catch (Exception ex)
            {
                // Handle exception (log, display error message, etc.)
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost("delete")]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            try
            {
                var getCarResponse = await _messageSession.Request<GetCarResponse>(new GetCarRequest(id)).ConfigureAwait(false);
                await _messageSession.Request<DeleteCarResponse>(new DeleteCarRequest(id)).ConfigureAwait(false);
                return RedirectToAction("Index", new { id = getCarResponse.Car.CompanyId });
            }
            catch (Exception ex)
            {
                // Handle exception (log, display error message, etc.)
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet("regnravailableasync")]
        public async Task<JsonResult> RegNrAvailableAsync(string regNr)
        {
            try
            {
                var getCarsResponse = await _messageSession.Request<GetCarsResponse>(new GetCarsRequest()).ConfigureAwait(false);
                bool isAvailable = getCarsResponse.Cars.All(c => c.RegNr != regNr);
                return Json(isAvailable);
            }
            catch (Exception ex)
            {
                // Handle exception (log, display error message, etc.)
                return Json(false);
            }
        }

        [HttpGet("vinavailableasync")]
        public async Task<JsonResult> VinAvailableAsync(string vin)
        {
            try
            {
                var getCarsResponse = await _messageSession.Request<GetCarsResponse>(new GetCarsRequest()).ConfigureAwait(false);
                bool isAvailable = getCarsResponse.Cars.All(c => c.VIN != vin);
                return Json(isAvailable);
            }
            catch (Exception ex)
            {
                // Handle exception (log, display error message, etc.)
                return Json(false);
            }
        }
    }
}
