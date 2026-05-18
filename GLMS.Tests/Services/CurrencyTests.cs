using GLMS.Services;
using System.Net.Http;
using Xunit;

namespace GLMS.Tests
{
    public class CurrencyTests
    {
        private readonly CurrencyService _currencyService;

        public CurrencyTests()
        {
            _currencyService =
                new CurrencyService(
                    new HttpClient());
        }

        // =========================
        // USD TEST
        // =========================

        [Fact]
        public async Task USD_Conversion_Returns_Valid_Rate()
        {
            decimal rate =
                await _currencyService
                .GetExchangeRate("USD");

            Assert.True(rate > 0);
        }

        // =========================
        // EUR TEST
        // =========================

        [Fact]
        public async Task EUR_Conversion_Returns_Valid_Rate()
        {
            decimal rate =
                await _currencyService
                .GetExchangeRate("EUR");

            Assert.True(rate > 0);
        }

        // =========================
        // GBP TEST
        // =========================

        [Fact]
        public async Task GBP_Conversion_Returns_Valid_Rate()
        {
            decimal rate =
                await _currencyService
                .GetExchangeRate("GBP");

            Assert.True(rate > 0);
        }

        // =========================
        // INVALID CURRENCY TEST
        // =========================

        
    }
}