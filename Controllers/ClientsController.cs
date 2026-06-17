using System.Net.Http.Json;
using GLMS.Models;
using Microsoft.AspNetCore.Mvc;

namespace GLMS.Controllers
{
    public class ClientsController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ClientsController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // GET: Clients
        public async Task<IActionResult> Index()
        {
            var httpClient = _httpClientFactory.CreateClient("GLMSAPI");

            var clients = await httpClient.GetFromJsonAsync<List<Client>>(
                "api/clients");

            return View(clients ?? new List<Client>());
        }

        // GET: Clients/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var httpClient = _httpClientFactory.CreateClient("GLMSAPI");

            var client = await httpClient.GetFromJsonAsync<Client>(
                $"api/clients/{id}");

            if (client == null)
            {
                return NotFound();
            }

            return View(client);
        }

        // GET: Clients/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Clients/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Client client)
        {
            if (!ModelState.IsValid)
            {
                return View(client);
            }

            var httpClient = _httpClientFactory.CreateClient("GLMSAPI");

            var response = await httpClient.PostAsJsonAsync(
                "api/Clients",
                client);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }

            // TEMPORARY: Show the API error in the browser
            var error = await response.Content.ReadAsStringAsync();
            return Content($"API Error: {response.StatusCode}\n\n{error}");
        }

        // GET: Clients/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var httpClient = _httpClientFactory.CreateClient("GLMSAPI");

            var client = await httpClient.GetFromJsonAsync<Client>(
                $"api/clients/{id}");

            if (client == null)
            {
                return NotFound();
            }

            return View(client);
        }

        // POST: Clients/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Client client)
        {
            if (id != client.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var httpClient = _httpClientFactory.CreateClient("GLMSAPI");

                var response = await httpClient.PutAsJsonAsync(
                    $"api/clients/{id}",
                    client);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }
            }

            return View(client);
        }

        // GET: Clients/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var httpClient = _httpClientFactory.CreateClient("GLMSAPI");

            var client = await httpClient.GetFromJsonAsync<Client>(
                $"api/clients/{id}");

            if (client == null)
            {
                return NotFound();
            }

            return View(client);
        }

        // POST: Clients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var httpClient = _httpClientFactory.CreateClient("GLMSAPI");

            await httpClient.DeleteAsync($"api/clients/{id}");

            return RedirectToAction(nameof(Index));
        }
    }
}