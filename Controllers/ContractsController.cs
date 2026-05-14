using GLMS.Data;
using GLMS.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace GLMS.Controllers
{
    public class ContractsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ContractsController(
            AppDbContext context,
            IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // ============================
        // INDEX
        // ============================

        public async Task<IActionResult> Index(
    string searchStatus,
    DateTime? startDate,
    DateTime? endDate)
        {
            var contracts = _context.Contracts
                .Include(c => c.Client)
                .AsQueryable();

            // FILTER BY STATUS

            if (!string.IsNullOrEmpty(searchStatus))
            {
                contracts = contracts.Where(c =>
                    c.Status == searchStatus);
            }

            // FILTER BY START DATE

            if (startDate.HasValue)
            {
                contracts = contracts.Where(c =>
                    c.StartDate >= startDate.Value);
            }

            // FILTER BY END DATE

            if (endDate.HasValue)
            {
                contracts = contracts.Where(c =>
                    c.EndDate <= endDate.Value);
            }

            return View(await contracts.ToListAsync());
        }

        // ============================
        // DETAILS
        // ============================

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contract = await _context.Contracts
                .Include(c => c.Client)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (contract == null)
            {
                return NotFound();
            }

            return View(contract);
        }

        // ============================
        // CREATE GET
        // ============================

        public IActionResult Create()
        {
            ViewData["ClientId"] = new SelectList(
                _context.Clients,
                "Id",
                "Name");

            return View();
        }

        // ============================
        // CREATE POST
        // ============================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Contract contract)
        {
            if (!ModelState.IsValid)
            {
                // ============================
                // PDF VALIDATION + SAVE
                // ============================

                if (contract.PdfFile != null)
                {
                    var extension =
                        Path.GetExtension(contract.PdfFile.FileName);

                    if (extension.ToLower() != ".pdf")
                    {
                        ModelState.AddModelError(
                            "",
                            "Only PDF files are allowed.");

                        ViewData["ClientId"] = new SelectList(
                            _context.Clients,
                            "Id",
                            "Name",
                            contract.ClientId);

                        return View(contract);
                    }

                    string uploadsFolder =
                        Path.Combine(
                            _webHostEnvironment.WebRootPath,
                            "uploads/contracts");

                    // CREATE FOLDER IF IT DOESN'T EXIST
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    // UNIQUE FILE NAME
                    string uniqueFileName =
                        Guid.NewGuid().ToString() + "_" +
                        contract.PdfFile.FileName;

                    string filePath =
                        Path.Combine(
                            uploadsFolder,
                            uniqueFileName);

                    // SAVE FILE
                    using (var fileStream =
                        new FileStream(filePath, FileMode.Create))
                    {
                        await contract.PdfFile.CopyToAsync(fileStream);
                    }

                    // SAVE PATH TO DATABASE
                    contract.PdfPath =
                        "/uploads/contracts/" + uniqueFileName;
                }

                _context.Add(contract);

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            ViewData["ClientId"] = new SelectList(
                _context.Clients,
                "Id",
                "Name",
                contract.ClientId);

            return View(contract);
        }

        // ============================
        // EDIT GET
        // ============================

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contract = await _context.Contracts.FindAsync(id);

            if (contract == null)
            {
                return NotFound();
            }

            ViewData["ClientId"] = new SelectList(
                _context.Clients,
                "Id",
                "Name",
                contract.ClientId);

            return View(contract);
        }

        // ============================
        // EDIT POST
        // ============================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            Contract contract)
        {
            if (id != contract.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // ============================
                    // PDF UPDATE
                    // ============================

                    if (contract.PdfFile != null)
                    {
                        var extension =
                            Path.GetExtension(
                                contract.PdfFile.FileName);

                        if (extension.ToLower() != ".pdf")
                        {
                            ModelState.AddModelError(
                                "",
                                "Only PDF files are allowed.");

                            ViewData["ClientId"] = new SelectList(
                                _context.Clients,
                                "Id",
                                "Name",
                                contract.ClientId);

                            return View(contract);
                        }

                        string uploadsFolder =
                            Path.Combine(
                                _webHostEnvironment.WebRootPath,
                                "uploads/contracts");

                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(
                                uploadsFolder);
                        }

                        string uniqueFileName =
                            Guid.NewGuid().ToString() + "_" +
                            contract.PdfFile.FileName;

                        string filePath =
                            Path.Combine(
                                uploadsFolder,
                                uniqueFileName);

                        using (var fileStream =
                            new FileStream(filePath,
                            FileMode.Create))
                        {
                            await contract.PdfFile
                                .CopyToAsync(fileStream);
                        }

                        contract.PdfPath =
                            "/uploads/contracts/" +
                            uniqueFileName;
                    }

                    _context.Update(contract);

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ContractExists(contract.Id))
                    {
                        return NotFound();
                    }

                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            ViewData["ClientId"] = new SelectList(
                _context.Clients,
                "Id",
                "Name",
                contract.ClientId);

            return View(contract);
        }

        // ============================
        // DELETE GET
        // ============================

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contract = await _context.Contracts
                .Include(c => c.Client)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (contract == null)
            {
                return NotFound();
            }

            return View(contract);
        }

        // ============================
        // DELETE POST
        // ============================

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var contract =
                await _context.Contracts.FindAsync(id);

            if (contract != null)
            {
                _context.Contracts.Remove(contract);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // ============================
        // CONTRACT EXISTS
        // ============================

        private bool ContractExists(int id)
        {
            return _context.Contracts
                .Any(e => e.Id == id);
        }
    }
}