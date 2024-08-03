using Client.Models;
using Client.Models.CarViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<CarController> _logger;

        public CarController(SignInManager<ApplicationUser> signInManager, IMessageSession messageSession, ILogger<CarController> logger)
        {
            _signInManager = signInManager;
            _messageSession = messageSession;
            _logger = logger;
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
                var filteredCars = getCarsResponse.Cars.Where(c => c.CompanyId == companyId).ToList();

                var selectList = getCompaniesResponse.Companies.Select(c => new SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString(),
                    Selected = c.Id == companyId
                }).ToList();

                var viewModel = new CarListViewModel(companyId)
                {
                    CompanySelectList = selectList,
                    Cars = filteredCars
                };

                ViewBag.CompanyId = companyId;
                ViewBag.CompanyName = selectedCompany?.Name;

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching car data");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("details/{id}")]
        public async Task<IActionResult> Details(Guid id)
        {
            try
            {
                var car = await GetCarByIdAsync(id).ConfigureAwait(false);
                var company = await GetCompanyByIdAsync(car.CompanyId).ConfigureAwait(false);

                ViewBag.CompanyName = company.Name;
                return View(car);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching car details");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("create/{companyId}")]
        public async Task<IActionResult> Create(Guid companyId)
        {
            try
            {
                var company = await GetCompanyByIdAsync(companyId).ConfigureAwait(false);
                ViewBag.CompanyName = company.Name;
                return View(new Car(companyId));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading create car view");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("create/{companyId}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CompanyId,VIN,RegNr,Online")] Car car)
        {
            if (!ModelState.IsValid) return View(car);

            try
            {
                await _messageSession.Request<CreateCarResponse>(new CreateCarRequest(car)).ConfigureAwait(false);
                return RedirectToAction("Index", new { id = car.CompanyId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating car");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("edit/{id}")]
        public async Task<IActionResult> Edit(Guid id)
        {
            try
            {
                var car = await GetCarByIdAsync(id).ConfigureAwait(false);
                car.Disabled = true; // Prevent updates of Online/Offline while editing

                var company = await GetCompanyByIdAsync(car.CompanyId).ConfigureAwait(false);
                ViewBag.CompanyName = company.Name;

                return View(car);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading edit car view");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Online")] Car car)
        {
            if (!ModelState.IsValid) return View(car);

            try
            {
                var oldCar = await GetCarByIdAsync(id).ConfigureAwait(false);
                oldCar.Online = car.Online;
                oldCar.Disabled = false; // Enable updates of Online/Offline when editing is done

                await _messageSession.Request<UpdateCarResponse>(new UpdateCarRequest(oldCar)).ConfigureAwait(false);
                return RedirectToAction("Index", new { id = oldCar.CompanyId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating car");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("delete/{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var car = await GetCarByIdAsync(id).ConfigureAwait(false);
                return View(car);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading delete car view");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            try
            {
                var car = await GetCarByIdAsync(id).ConfigureAwait(false);
                await _messageSession.Request<DeleteCarResponse>(new DeleteCarRequest(id)).ConfigureAwait(false);
                return RedirectToAction("Index", new { id = car.CompanyId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting car");
                return StatusCode(500, "Internal server error");
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
                _logger.LogError(ex, "Error checking RegNr availability");
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
                _logger.LogError(ex, "Error checking VIN availability");
                return Json(false);
            }
        }

        private async Task<Car> GetCarByIdAsync(Guid id)
        {
            var response = await _messageSession.Request<GetCarResponse>(new GetCarRequest(id)).ConfigureAwait(false);
            return response.Car;
        }

        private async Task<Company> GetCompanyByIdAsync(Guid id)
        {
            var response = await _messageSession.Request<GetCompanyResponse>(new GetCompanyRequest(id)).ConfigureAwait(false);
            return response.Company;
        }
    }
}
