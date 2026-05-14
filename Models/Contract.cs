using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace GLMS.Models
{
    public class Contract
    {
        public int Id { get; set; }

        public int ClientId { get; set; }

        public Client Client { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string Status { get; set; }

        public string ServiceLevel { get; set; }

        public string? PdfPath { get; set; }

        [NotMapped]
        public IFormFile? PdfFile { get; set; }

        public ICollection<ServiceRequest>? ServiceRequests { get; set; }
    }
}