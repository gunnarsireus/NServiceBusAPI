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
    using Shared.Requests;
    using Shared.Responses;

    [Route("company")]
    public class CompanyController : Controller
    {
        readonly SignInManager<ApplicationUser> _signInManager;
        readonly IMessageSession _messageSession;

        public CompanyController(SignInManager<ApplicationUser> signInManager, IMessageSession messageSession)
        {
            _signInManager = signInManager;
            _messageSession = messageSession;
        }


        [HttpGet("index")]
        public async Task<IActionResult> Index()
        {
            if (!_signInManager.IsSignedIn(User)) return RedirectToAction("Index", "Home");
            var getCompaniesResponse = await _messageSession.Request<GetCompaniesResponse>(new GetCompaniesRequest());
            var companies = getCompaniesResponse.Companies;

            var getCarsResponse = await _messageSession.Request<GetCarsResponse>(new GetCarsRequest());
            var allCars = getCarsResponse.Cars;

            foreach (var company in companies)
            {

                var cars = allCars.Where(c => c.CompanyId == company.Id).ToList();
                company.Cars = cars;
            }

            var companyViewModel = new CompanyViewModel { Companies = companies }; ;

            return View(companyViewModel);
        }

        [HttpGet("details/{id}")]
        public async Task<IActionResult> Details(Guid id)
        {
            var getCompanyResponse = await _messageSession.Request<GetCompanyResponse>(new GetCompanyRequest(id));
            var company = getCompanyResponse.Company;

            return View(company);
        }

        [HttpGet("create")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Company/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Address,CreationTime")] Company company)
        {
            if (!ModelState.IsValid) return View(company);
            company.Id = Guid.NewGuid();
            var createCompanyResponse = await _messageSession.Request<CreateCompanyResponse>(new CreateCompanyRequest(company));

            return RedirectToAction(nameof(Index));
        }

        [HttpGet("edit/{id}")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var getCompanyResponse = await _messageSession.Request<GetCompanyResponse>(new GetCompanyRequest(id));
            var company = getCompanyResponse.Company;

            return View(company);
        }

        // POST: Company/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,CreationTime, Name, Address")] Company company)
        {
            if (!ModelState.IsValid) return View(company);

            var getCompanyResponse = await _messageSession.Request<GetCompanyResponse>(new GetCompanyRequest(id));
            var oldCompany = getCompanyResponse.Company;

            oldCompany.Name = company.Name;
            oldCompany.Address = company.Address;
            var updateCompanyResponse = await _messageSession.Request<UpdateCompanyResponse>(new UpdateCompanyRequest(oldCompany));

            return RedirectToAction(nameof(Index));
        }

        [HttpGet("delete/{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var getCompanyResponse = await _messageSession.Request<GetCompanyResponse>(new GetCompanyRequest(id));
            var company = getCompanyResponse.Company;

            return View(company);
        }

        // POST: Company/Delete/5
        [HttpPost("delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            await _messageSession.Request<DeleteCompanyResponse>(new DeleteCompanyRequest(id));

            return RedirectToAction(nameof(Index));
        }
    }
}