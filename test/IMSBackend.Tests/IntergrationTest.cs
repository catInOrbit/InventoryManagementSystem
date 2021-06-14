//
// using System.Net.Http;
// using InventoryManagementSystem.PublicApi;
// using Microsoft.AspNetCore.Hosting;
// using Microsoft.AspNetCore.TestHost;
// using Xunit;
//
// namespace IMSBackend.Tests
// {
//     public class RequisitionEndpointsTest
//     {
//         private readonly HttpClient _client;
//
//         public RequisitionEndpointsTest()
//         {
//             var server = new TestServer(new WebHostBuilder()
//                 .UseEnvironment("Development")
//                 .UseStartup<Startup>());
//             _client = server.CreateClient();
//         }
//
//         [Fact]
//         public void RequisitionCreate()
//         {
//             var request = new HttpRequestMessage(new HttpMethod("/api/"), )
//             
//         }
//     }
// }