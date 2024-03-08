using Client.Models;
using Client.Models.CompanyViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Shared.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CarClient.Controllers
{
    using NServiceBus;

    [Route("/company")]
    public class CompanyController : Controller
    {
        readonly SignInManager<ApplicationUser> _signInManager;
        readonly IMessageSession _messageSession;

        public CompanyController(SignInManager<ApplicationUser> signInManager, IMessageSession messageSession)
        {
            _signInManager = signInManager;
            _messageSession = messageSession;
        }


        [HttpGet("/company")]

        public async Task<IActionResult> Index()
        {
            if (!_signInManager.IsSignedIn(User)) return RedirectToAction("Index", "Home");
            var getCompaniesResponse = await Client.Utils.Utils.GetCompaniesResponseAsync(_messageSession);
            var companies = getCompaniesResponse.Companies;

            foreach (var company in companies)
            {
                var getCarsResponse = await Client.Utils.Utils.GetCarsResponseAsync(_messageSession);
                var cars = getCarsResponse.Cars;
                cars = cars.Where(c => c.CompanyId == company.Id).ToList();
                company.Cars = cars;
            }

            var companyViewModel = new CompanyViewModel { Companies = companies };

            return View(companyViewModel);
        }


        [HttpGet("/company/details")]
        public async Task<IActionResult> Details(Guid id)
        {
            var getCompanyResponse = await Client.Utils.Utils.GetCompanyResponseAsync(id, _messageSession);
            var company = getCompanyResponse.Company;

            return View(company);
        }

        [HttpGet("/company/create")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Company/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("/company/create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Address,CreationTime")] Company company)
        {
            if (!ModelState.IsValid) return View(company);
            company.Id = Guid.NewGuid();
            var createCompanyResponse = await Client.Utils.Utils.CreateCompanyResponseAsync(company, _messageSession);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet("/company/edit")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var getCompanyResponse = await Client.Utils.Utils.GetCompanyResponseAsync(id, _messageSession);
            var company = getCompanyResponse.Company;

            return View(company);
        }

        // POST: Company/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("/company/edit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,CreationTime, Name, Address")] Company company)
        {
            if (!ModelState.IsValid) return View(company);

            var getCompanyResponse = await Client.Utils.Utils.GetCompanyResponseAsync(id, _messageSession);
            var oldCompany = getCompanyResponse.Company;

            oldCompany.Name = company.Name;
            oldCompany.Address = company.Address;
            var updateCompanyResponse = await Client.Utils.Utils.UpdateCompanyResponseAsync(company, _messageSession);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet("/company/delete")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var getCompanyResponse = await Client.Utils.Utils.GetCompanyResponseAsync(id, _messageSession);
            var company = getCompanyResponse.Company;

            return View(company);
        }

        // POST: Company/Delete/5
        [HttpPost("/company/delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var deleteCompanyResponse = await Client.Utils.Utils.DeleteCompanyResponseAsync(id, _messageSession);

            return RedirectToAction(nameof(Index));
        }
    }
}