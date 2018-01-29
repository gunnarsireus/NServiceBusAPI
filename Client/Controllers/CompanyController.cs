using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Client.Models;
using Client.Models.CompanyViewModel;
using Shared.Models;

namespace CarClient.Controllers
{
    using Client.DAL;
    using NServiceBus;
    using Shared.Requests;

    public class CompanyController : Controller
	{
		readonly SignInManager<ApplicationUser> _signInManager;
		readonly IEndpointInstance _endpointInstance;
        readonly CarDataAccess _dataAccess;

        public CompanyController(SignInManager<ApplicationUser> signInManager, IEndpointInstance endpointInstance, CarApiContext carApiContext)
		{
			_signInManager = signInManager;
			_endpointInstance = endpointInstance;
            _dataAccess = new CarDataAccess(carApiContext);
        }


		// GET: Company

		public async Task<IActionResult> Index()
		{
			if (!_signInManager.IsSignedIn(User)) return RedirectToAction("Index", "Home");
			var companies = _dataAccess.GetCompanies().ToList();

			foreach (var company in companies)
			{
				var cars = _dataAccess.GetCars();
				cars = cars.Where(c => c.CompanyId == company.Id).ToList();
				company.Cars = cars;
			}

			var companyViewModel = new CompanyViewModel { Companies = companies };

			return View(companyViewModel);
		}

		// GET: Company/Details/5
		public async Task<IActionResult> Details(Guid id)
		{
			var company = _dataAccess.GetCompany(id);

			return View(company);
		}

		// GET: Company/Create
		public IActionResult Create()
		{
			return View();
		}

		// POST: Company/Create
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create([Bind("Name,Address,CreationTime")] Company company)
		{
			if (!ModelState.IsValid) return View(company);
			company.Id = Guid.NewGuid();
            var message = new CreateCompanyRequest(company);
            await _endpointInstance.Send(message).ConfigureAwait(false);

            return RedirectToAction(nameof(Index));
		}

		// GET: Company/Edit/5
		public async Task<IActionResult> Edit(Guid id)
		{
			var company = _dataAccess.GetCompany(id);
			return View(company);
		}

		// POST: Company/Edit/5
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(Guid id, [Bind("Id,CreationTime, Name, Address")] Company company)
		{
			if (!ModelState.IsValid) return View(company);
			var oldCompany = _dataAccess.GetCompany(id);
			oldCompany.Name = company.Name;
			oldCompany.Address = company.Address;
            var message = new UpdateCompanyRequest(oldCompany);
            await _endpointInstance.Send(message).ConfigureAwait(false);

            return RedirectToAction(nameof(Index));
		}

		// GET: Company/Delete/5
		public async Task<IActionResult> Delete(Guid id)
		{
			var company = _dataAccess.GetCompany(id);
			return View(company);
		}

		// POST: Company/Delete/5
		[HttpPost]
		[ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(Guid id)
		{
            var message = new DeleteCompanyRequest(id);
            await _endpointInstance.Send(message).ConfigureAwait(false);
            return RedirectToAction(nameof(Index));
		}
	}
}