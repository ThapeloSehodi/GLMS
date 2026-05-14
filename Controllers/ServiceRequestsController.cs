using GLMS.Data;
using GLMS.Models;
using GLMS.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace GLMS.Controllers
{
    public class ServiceRequestsController : Controller
    {
        private readonly AppDbContext _context;

        private readonly CurrencyService _currencyService;

        public ServiceRequestsController(
     AppDbContext context,
     CurrencyService currencyService)
        {
            _context = context;
            _currencyService = currencyService;
        }

        // =========================================
        // INDEX
        // =========================================

        public async Task<IActionResult> Index()
        {
            var serviceRequests = _context.ServiceRequests
                .Include(s => s.Contract);

            return View(await serviceRequests.ToListAsync());
        }

        // =========================================
        // DETAILS
        // =========================================

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var serviceRequest = await _context.ServiceRequests
                .Include(s => s.Contract)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (serviceRequest == null)
            {
                return NotFound();
            }

            return View(serviceRequest);
        }

        // =========================================
        // CREATE GET
        // =========================================

        public IActionResult Create()
        {
            var activeContracts = _context.Contracts
                .Where(c => c.Status == "Active")
                .ToList();

            ViewData["ContractId"] = new SelectList(
                activeContracts,
                "Id",
                "Id");

            return View();
        }


        // CREATE POST
        // ASYNC/AWAIT is used to improve application
        // responsiveness when consuming external APIs
        // and accessing the database.

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
    ServiceRequest serviceRequest)
        {
            var contract = await _context.Contracts
                .FindAsync(serviceRequest.ContractId);

            // WORKFLOW VALIDATION

            if (contract == null)
            {
                ModelState.AddModelError(
                    "",
                    "Selected contract does not exist.");
            }
            else if (contract.Status != "Active")
            {
                ModelState.AddModelError(
                    "",
                    "Service requests can only be created for active contracts.");
            }

            // API CONVERSION

            decimal exchangeRate =
                await _currencyService.GetUsdToZarRate();

            serviceRequest.CostZAR =
                serviceRequest.CostUSD * exchangeRate;

            if (!ModelState.IsValid)
            {
                _context.Add(serviceRequest);

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            ViewData["ContractId"] = new SelectList(
                _context.Contracts
                .Where(c => c.Status == "Active"),
                "Id",
                "Id",
                serviceRequest.ContractId);

            return View(serviceRequest);
        }

        // =========================================
        // EDIT GET
        // =========================================

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var serviceRequest =
                await _context.ServiceRequests.FindAsync(id);

            if (serviceRequest == null)
            {
                return NotFound();
            }

            ViewData["ContractId"] = new SelectList(
                _context.Contracts,
                "Id",
                "Id",
                serviceRequest.ContractId);

            return View(serviceRequest);
        }

        // =========================================
        // EDIT POST
        // =========================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            ServiceRequest serviceRequest)
        {
            if (id != serviceRequest.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(serviceRequest);

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ServiceRequestExists(
                        serviceRequest.Id))
                    {
                        return NotFound();
                    }

                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            ViewData["ContractId"] = new SelectList(
                _context.Contracts,
                "Id",
                "Id",
                serviceRequest.ContractId);

            return View(serviceRequest);
        }

        // =========================================
        // DELETE GET
        // =========================================

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var serviceRequest =
                await _context.ServiceRequests
                .Include(s => s.Contract)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (serviceRequest == null)
            {
                return NotFound();
            }

            return View(serviceRequest);
        }

        // =========================================
        // DELETE POST
        // =========================================

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(
            int id)
        {
            var serviceRequest =
                await _context.ServiceRequests
                .FindAsync(id);

            if (serviceRequest != null)
            {
                _context.ServiceRequests
                    .Remove(serviceRequest);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // =========================================
        // EXISTS
        // =========================================

        private bool ServiceRequestExists(int id)
        {
            return _context.ServiceRequests
                .Any(e => e.Id == id);
        }
    }
}