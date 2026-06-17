using System.Text.Json.Serialization;

namespace GLMS.API.Models
{
    public class ServiceRequest
    {
        public int Id { get; set; }

        public int ContractId { get; set; }

        [JsonIgnore]
        public Contract? Contract { get; set; }

        public string Description { get; set; }

        public decimal ForeignAmount { get; set; }

        public string CurrencyType { get; set; }

        public decimal? CostZAR { get; set; }

        public string Status { get; set; }
    }
}
