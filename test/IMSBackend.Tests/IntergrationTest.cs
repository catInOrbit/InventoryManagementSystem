
using System.Net.Http;
using System.Security.Policy;
using System.Threading.Tasks;
using InventoryManagementSystem.PublicApi;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Xunit;

namespace IMSBackend.Tests
{
    public class RequisitionEndpointsTest : IClassFixture<TestFixture<Startup>>
    {
        private readonly HttpClient _client;

        public RequisitionEndpointsTest(TestFixture<Startup> fixture)
        {
            _client = fixture.Client;
        }

        private static string localHostString = "http://0.0.0.0:40927";
        [Fact]
        public async Task RequisitionCreate()
        {
            var request = new
            {
                Url = "/api/requisition/create",
            };
            
            var response = await _client.PostAsync(request.Url, ContentHelper.GetStringContent(""));
            response.EnsureSuccessStatusCode();
        }
    }
}