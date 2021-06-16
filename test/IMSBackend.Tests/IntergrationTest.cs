
using System.Net.Http;
using System.Security.Policy;
using System.Threading.Tasks;
using InventoryManagementSystem.PublicApi;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Xunit;

namespace IMSBackend.Tests
{
    public class EndpointsTest : IClassFixture<TestFixture<Startup>>
    {
        private readonly HttpClient _client;
        private string authorization;

        public EndpointsTest(TestFixture<Startup> fixture)
        {
            // Arrange
            _client = fixture.Client;
        }
        
        [Fact]
        private async Task Authorization()
        {
            var request = new
            {
                Url = "/api/authentication",
                Body = new
                {
                    Email = "tmh1799@gmail.com",
                    Password = "test@12345Aha"
                },
            };
            
            var response = await _client.PostAsync(request.Url, ContentHelper.GetStringContent(request.Body));
            var value = await response.Content.ReadAsStringAsync();
            var indexFound = value.IndexOf(": ");
            var token = value.Substring(value.IndexOf("token: "));
        }

        [Fact]
        private async Task CreateRequisitionTest()
        {
            var request = new
            {
                Url = "/api/requisition/create",
                Body = new
                {
                
                },
            };
            
            var response = await _client.PostAsync(request.Url, ContentHelper.GetStringContent(request.Body));
            var value = await response.Content.ReadAsStringAsync();
        }
    }

}