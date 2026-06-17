using Newtonsoft.Json;
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

        public async Task<decimal> GetExchangeRate(string currency)
        {
            using var client = new HttpClient();

            string url =
                $"https://open.er-api.com/v6/latest/{currency}";

            var response =
                await client.GetStringAsync(url);

            dynamic data =
                JsonConvert.DeserializeObject(response);

            decimal zarRate =
                data.rates.ZAR;

            return zarRate;
        }
    }
}