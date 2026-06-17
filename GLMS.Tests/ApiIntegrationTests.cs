using System.Net;
using System.Net.Http;
using Xunit;

namespace GLMS.Tests
{
    public class ApiIntegrationTests
    {
        private readonly HttpClient _client;

        public ApiIntegrationTests()
        {
            _client = new HttpClient();

            _client.BaseAddress =
                new Uri("http://localhost:5181/");
        }

        [Fact]
        public async Task GetContracts_Returns_OK()
        {
            var response =
                await _client.GetAsync("api/contracts");

            Assert.Equal(
                HttpStatusCode.OK,
                response.StatusCode);
        }

        [Fact]
        public async Task GetClients_Returns_OK()
        {
            var response =
                await _client.GetAsync("api/clients");

            Assert.Equal(
                HttpStatusCode.OK,
                response.StatusCode);
        }

        [Fact]
        public async Task GetServiceRequests_Returns_OK()
        {
            var response =
                await _client.GetAsync("api/servicerequests");

            Assert.Equal(
                HttpStatusCode.OK,
                response.StatusCode);
        }
    }
}