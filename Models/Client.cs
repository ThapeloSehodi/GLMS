using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;

namespace GLMS.Models
{
    public class Client
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string ContactDetails { get; set; }

        public string Region { get; set; }

        public ICollection<Contract> Contracts { get; set; }
    }
}