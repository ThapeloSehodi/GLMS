using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace GLMS.API.Models
{
    public class Client
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string ContactDetails { get; set; }

        public string Region { get; set; }

        [JsonIgnore]
        public ICollection<Contract> Contracts { get; set; } = new List<Contract>();
    }
}