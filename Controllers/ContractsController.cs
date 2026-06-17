using System.Net.Http.Json;
using GLMS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GLMS.Controllers
{
    public class ContractsController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ContractsController(
            IHttpClientFactory httpClientFactory,
            IWebHostEnvironment webHostEnvironment)
        {
            _httpClientFactory = httpClientFactory;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index(
            string searchStatus,
            DateTime? startDate,
            DateTime? endDate)
        {
            var client = _httpClientFactory.CreateClient("GLMSAPI");

            var response = await client.GetAsync("api/contracts");

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                return Content($"API Error: {response.StatusCode}\n\n{error}");
            }

            var contracts = await response.Content.ReadFromJsonAsync<List<Contract>>();

            contracts ??= new List<Contract>();

            if (!string.IsNullOrEmpty(searchStatus))
                contracts = contracts.Where(c => c.Status == searchStatus).ToList();

            if (startDate.HasValue)
                contracts = contracts.Where(c => c.StartDate >= startDate.Value).ToList();

            if (endDate.HasValue)
                contracts = contracts.Where(c => c.EndDate <= endDate.Value).ToList();

            return View(contracts);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var client = _httpClientFactory.CreateClient("GLMSAPI");

            var contract = await client.GetFromJsonAsync<Contract>(
                $"api/contracts/{id}");

            if (contract == null) return NotFound();

            return View(contract);
        }

        public async Task<IActionResult> Create()
        {
            await LoadClientsDropdown();
            return View();
        }

       [HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Create(Contract contract)
{
    // Force form values into model
    contract.Status = Request.Form["Status"];
    contract.ServiceLevel = Request.Form["ServiceLevel"];

    if (contract.PdfFile != null)
    {
        var extension = Path.GetExtension(contract.PdfFile.FileName);

        if (extension.ToLower() != ".pdf")
        {
            ModelState.AddModelError("", "Only PDF files are allowed.");
            await LoadClientsDropdown(contract.ClientId);
            return View(contract);
        }

        string uploadsFolder = Path.Combine(
            _webHostEnvironment.WebRootPath,
            "uploads/contracts");

        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }

        string uniqueFileName =
            Guid.NewGuid().ToString() + "_" +
            contract.PdfFile.FileName;

        string filePath =
            Path.Combine(uploadsFolder, uniqueFileName);

        using (var fileStream =
            new FileStream(filePath, FileMode.Create))
        {
            await contract.PdfFile.CopyToAsync(fileStream);
        }

        contract.PdfPath =
            "/uploads/contracts/" + uniqueFileName;
    }

    contract.PdfFile = null;

    if (string.IsNullOrWhiteSpace(contract.Status))
    {
        return Content("MVC Error: Status is empty.");
    }

    if (string.IsNullOrWhiteSpace(contract.ServiceLevel))
    {
        return Content("MVC Error: ServiceLevel is empty.");
    }

    var client = _httpClientFactory.CreateClient("GLMSAPI");

    var response = await client.PostAsJsonAsync(
        "api/contracts",
        contract);

    if (response.IsSuccessStatusCode)
    {
        return RedirectToAction(nameof(Index));
    }

    var error = await response.Content.ReadAsStringAsync();

    return Content(
        $"API Error: {response.StatusCode}\n\n{error}");
}

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var client = _httpClientFactory.CreateClient("GLMSAPI");

            var contract = await client.GetFromJsonAsync<Contract>(
                $"api/contracts/{id}");

            if (contract == null) return NotFound();

            await LoadClientsDropdown(contract.ClientId);
            return View(contract);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Contract contract)
        {
            if (id != contract.Id) return NotFound();

            var client = _httpClientFactory.CreateClient("GLMSAPI");

            var existingContract = await client.GetFromJsonAsync<Contract>(
                $"api/contracts/{id}");

            if (existingContract == null) return NotFound();

            if (contract.PdfFile == null)
            {
                contract.PdfPath = existingContract.PdfPath;
            }
            else
            {
                var extension = Path.GetExtension(contract.PdfFile.FileName);

                if (extension.ToLower() != ".pdf")
                {
                    ModelState.AddModelError("", "Only PDF files are allowed.");
                    await LoadClientsDropdown(contract.ClientId);
                    return View(contract);
                }

                string uploadsFolder = Path.Combine(
                    _webHostEnvironment.WebRootPath,
                    "uploads/contracts");

                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                string uniqueFileName =
                    Guid.NewGuid().ToString() + "_" + contract.PdfFile.FileName;

                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await contract.PdfFile.CopyToAsync(fileStream);
                }

                contract.PdfPath = "/uploads/contracts/" + uniqueFileName;
            }

            contract.PdfFile = null;

            var response = await client.PutAsJsonAsync(
                $"api/contracts/{id}",
                contract);

            if (response.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index));

            var error = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError("", $"API error: {error}");

            await LoadClientsDropdown(contract.ClientId);
            return View(contract);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var client = _httpClientFactory.CreateClient("GLMSAPI");

            var contract = await client.GetFromJsonAsync<Contract>(
                $"api/contracts/{id}");

            if (contract == null) return NotFound();

            return View(contract);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var client = _httpClientFactory.CreateClient("GLMSAPI");

            await client.DeleteAsync($"api/contracts/{id}");

            return RedirectToAction(nameof(Index));
        }

        private async Task LoadClientsDropdown(int? selectedClientId = null)
        {
            var client = _httpClientFactory.CreateClient("GLMSAPI");

            var clients = await client.GetFromJsonAsync<List<Client>>(
                "api/clients");

            ViewData["ClientId"] = new SelectList(
                clients ?? new List<Client>(),
                "Id",
                "Name",
                selectedClientId);
        }
    }
}