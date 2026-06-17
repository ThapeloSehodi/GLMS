using System.Net.Http.Json;
using GLMS.Models;
using GLMS.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GLMS.Controllers
{
    public class ServiceRequestsController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly CurrencyService _currencyService;

        public ServiceRequestsController(
            IHttpClientFactory httpClientFactory,
            CurrencyService currencyService)
        {
            _httpClientFactory = httpClientFactory;
            _currencyService = currencyService;
        }

        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient("GLMSAPI");

            var response = await client.GetAsync("api/servicerequests");

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                return Content($"API Error: {response.StatusCode}\n\n{error}");
            }

            var serviceRequests =
                await response.Content.ReadFromJsonAsync<List<ServiceRequest>>();

            return View(serviceRequests ?? new List<ServiceRequest>());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var client = _httpClientFactory.CreateClient("GLMSAPI");

            var serviceRequest =
                await client.GetFromJsonAsync<ServiceRequest>(
                    $"api/servicerequests/{id}");

            if (serviceRequest == null)
                return NotFound();

            return View(serviceRequest);
        }

        public async Task<IActionResult> Create()
        {
            await LoadActiveContractsDropdown();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ServiceRequest serviceRequest)
        {
            var client = _httpClientFactory.CreateClient("GLMSAPI");

            // Force form values into model
            serviceRequest.ContractId = int.Parse(Request.Form["ContractId"]);
            serviceRequest.Description = Request.Form["Description"].ToString();
            serviceRequest.CurrencyType = Request.Form["CurrencyType"].ToString();
            serviceRequest.Status = Request.Form["Status"].ToString();

            if (decimal.TryParse(Request.Form["ForeignAmount"], out decimal foreignAmount))
            {
                serviceRequest.ForeignAmount = foreignAmount;
            }

            var contract = await client.GetFromJsonAsync<Contract>(
                $"api/contracts/{serviceRequest.ContractId}");

            if (contract == null)
            {
                ModelState.AddModelError("", "Selected contract does not exist.");
            }
            else if (contract.Status != "Active")
            {
                ModelState.AddModelError(
                    "",
                    "Service requests can only be created for active contracts.");
            }

            decimal rate =
                await _currencyService.GetExchangeRate(
                    serviceRequest.CurrencyType);

            serviceRequest.CostZAR =
                serviceRequest.ForeignAmount * rate;

            if (ModelState.IsValid)
            {
                await LoadActiveContractsDropdown(serviceRequest.ContractId);
                return View(serviceRequest);
            }

            var apiServiceRequest = new
            {
                contractId = serviceRequest.ContractId,
                description = serviceRequest.Description,
                foreignAmount = serviceRequest.ForeignAmount,
                currencyType = serviceRequest.CurrencyType,
                costZAR = serviceRequest.CostZAR,
                status = serviceRequest.Status
            };

            var response = await client.PostAsJsonAsync(
                "api/servicerequests",
                apiServiceRequest);

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
            if (id == null)
                return NotFound();

            var client = _httpClientFactory.CreateClient("GLMSAPI");

            var serviceRequest =
                await client.GetFromJsonAsync<ServiceRequest>(
                    $"api/servicerequests/{id}");

            if (serviceRequest == null)
                return NotFound();

            await LoadActiveContractsDropdown(serviceRequest.ContractId);
            return View(serviceRequest);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            ServiceRequest serviceRequest)
        {
            if (id != serviceRequest.Id)
                return NotFound();

            var client = _httpClientFactory.CreateClient("GLMSAPI");

            var contract =
                await client.GetFromJsonAsync<Contract>(
                    $"api/contracts/{serviceRequest.ContractId}");

            if (contract == null)
            {
                ModelState.AddModelError("", "Contract not found.");
            }
            else if (contract.Status != "Active")
            {
                ModelState.AddModelError(
                    "",
                    "Only active contracts can be edited.");
            }

            decimal rate =
                await _currencyService.GetExchangeRate(
                    serviceRequest.CurrencyType);

            serviceRequest.CostZAR =
                serviceRequest.ForeignAmount * rate;

            if (!ModelState.IsValid)
            {
                await LoadActiveContractsDropdown(serviceRequest.ContractId);
                return View(serviceRequest);
            }

            serviceRequest.Contract = null;

            var response = await client.PutAsJsonAsync(
                $"api/servicerequests/{id}",
                serviceRequest);

            if (response.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index));

            var error = await response.Content.ReadAsStringAsync();
            return Content($"API Error: {response.StatusCode}\n\n{error}");
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var client = _httpClientFactory.CreateClient("GLMSAPI");

            var serviceRequest =
                await client.GetFromJsonAsync<ServiceRequest>(
                    $"api/servicerequests/{id}");

            if (serviceRequest == null)
                return NotFound();

            return View(serviceRequest);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var client = _httpClientFactory.CreateClient("GLMSAPI");

            await client.DeleteAsync($"api/servicerequests/{id}");

            return RedirectToAction(nameof(Index));
        }

        private async Task LoadActiveContractsDropdown(
            int? selectedContractId = null)
        {
            var client = _httpClientFactory.CreateClient("GLMSAPI");

            var contracts =
                await client.GetFromJsonAsync<List<Contract>>(
                    "api/contracts?status=Active");

            ViewData["ContractId"] = new SelectList(
                contracts ?? new List<Contract>(),
                "Id",
                "Id",
                selectedContractId);
        }
    }
}