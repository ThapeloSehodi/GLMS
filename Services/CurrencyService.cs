using Newtonsoft.Json.Linq;

namespace GLMS.Services
{
    public class CurrencyService
    {
        private readonly HttpClient _httpClient;

        public CurrencyService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<decimal> GetUsdToZarRate()
        {
            string url =
                "https://open.er-api.com/v6/latest/USD";

            var response =
                await _httpClient.GetStringAsync(url);

            var data = JObject.Parse(response);

            decimal zarRate =
                data["rates"]["ZAR"].Value<decimal>();

            return zarRate;
        }
    }
}