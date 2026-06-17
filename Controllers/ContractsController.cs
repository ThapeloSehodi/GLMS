using GLMS.API.Data;
using GLMS.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;


namespace GLMS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContractsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ContractsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Contract>>> GetContracts(
            string? status,
            DateTime? startDate,
            DateTime? endDate)
        {
            var query = _context.Contracts
                .Include(c => c.Client)
                .AsQueryable();

            if (!string.IsNullOrEmpty(status))
                query = query.Where(c => c.Status == status);

            if (startDate.HasValue)
                query = query.Where(c => c.StartDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(c => c.EndDate <= endDate.Value);

            return await query.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Contract>> GetContract(int id)
        {
            var contract = await _context.Contracts
                .Include(c => c.Client)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (contract == null)
                return NotFound();

            return contract;
        }

        [HttpPost]
        public async Task<ActionResult<Contract>> CreateContract(JsonElement data)
        {
            int clientId = data.GetProperty("clientId").GetInt32();
            DateTime startDate = data.GetProperty("startDate").GetDateTime();
            DateTime endDate = data.GetProperty("endDate").GetDateTime();
            string status = data.GetProperty("status").GetString();
            string serviceLevel = data.GetProperty("serviceLevel").GetString();
            string? pdfPath = null;

            if (data.TryGetProperty("pdfPath", out JsonElement pdfElement))
            {
                pdfPath = pdfElement.GetString();
            }

            if (string.IsNullOrWhiteSpace(serviceLevel))
            {
                return BadRequest("ServiceLevel is required.");
            }

            if (string.IsNullOrWhiteSpace(status))
            {
                return BadRequest("Status is required.");
            }

            var newContract = new Contract
            {
                ClientId = clientId,
                StartDate = startDate,
                EndDate = endDate,
                Status = status,
                ServiceLevel = serviceLevel,
                PdfPath = pdfPath
            };

            _context.Contracts.Add(newContract);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetContract),
                new { id = newContract.Id },
                newContract);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateContract(int id, Contract contract)
        {
            if (id != contract.Id)
                return BadRequest();

            contract.PdfFile = null;

            _context.Entry(contract).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatus(
            int id,
            [FromBody] string status)
        {
            var contract = await _context.Contracts.FindAsync(id);

            if (contract == null)
                return NotFound();

            contract.Status = status;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContract(int id)
        {
            var contract = await _context.Contracts.FindAsync(id);

            if (contract == null)
                return NotFound();

            _context.Contracts.Remove(contract);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}