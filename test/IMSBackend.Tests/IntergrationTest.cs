//
// using System;
// using System.Collections.Generic;
// using System.IO;
// using System.Linq;
// using System.Net.Http;
// using System.Net.Http.Headers;
// using System.Text;
// using System.Threading.Tasks;
// using InventoryManagementSystem.PublicApi;
// using InventoryManagementSystem.PublicApi.ReceivingOrderEndpoints;
// using Newtonsoft.Json;
// using Newtonsoft.Json.Linq;
// using Xunit;
// using Xunit.Abstractions;
// using JsonSerializer = System.Text.Json.JsonSerializer;
//
// namespace IMSBackend.Tests
// {
//     public class EndpointsTest : IClassFixture<TestFixture<Startup>>
//     {
//         private readonly ITestOutputHelper _testOutputHelper;
//         private readonly HttpClient _client;
//         public EndpointsTest(TestFixture<Startup> fixture, ITestOutputHelper testOutputHelper)
//         {
//             _testOutputHelper = testOutputHelper;
//             _client = fixture.Client;
//         }
//
//         private string tokenBearer = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6IjkyZThlZGFjLWFkNTQtNGFlNi1hZTIyLTBlMGM1MDJkYTYxMSIsIm5iZiI6MTYyMzk1NDcwNywiZXhwIjoxNjI0NTU5NTA3LCJpYXQiOjE2MjM5NTQ3MDd9.3ODV-egYmxK6NkdAdJ6VI3qLNBEctwldRi45QfRyEOE";
//         
//         [Fact]
//         private async Task Authorization()
//         {
//             var request = new
//             {
//                 Url = "/api/authentication",
//                 Body = new
//                 {
//                     Email = "tmh1799@gmail.com",
//                     Password = "test@12345Aha"
//                 },
//             };
//             
//             var response = await _client.PostAsync(request.Url, ContentHelper.GetStringContent(request.Body));
//             response.EnsureSuccessStatusCode();
//             
//             var value = await response.Content.ReadAsStringAsync();
//             String[] seperator = {":", ","};
//             var token = value.Split(seperator,7, StringSplitOptions.RemoveEmptyEntries);
//             tokenBearer = token[5];
//             // System.Environment.SetEnvironmentVariable("token", tokenBearer);
//             _testOutputHelper.WriteLine(tokenBearer);
//             // _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6ImM3ODY3NmY2LTc1NTUtNGU3ZS05OWQ5LWE4OTcxZGI4NWU5MiIsIm5iZiI6MTYyMzg2NTA0MCwiZXhwIjoxNjI0NDY5ODQwLCJpYXQiOjE2MjM4NjUwNDB9.R4PdfhLqs4z-zOpMiCMXyD2xYm5UXNTPR_AdkkASLSw");        
//         }
//
//         [Fact]
//         private async Task UpdateGoodsReceipt()
//         {
//             var request = new HttpRequestMessage(HttpMethod.Put, "api/goodsreceipt/update");
//             request.Content = new StringContent(JsonSerializer.Serialize(new
//             {
//                 purchaseOrderNumber = "PO6XW9M1Y4",
//                 storageLocation = "Back",
//                 updateItems = new List<ROItemUpdateRequest>
//                 {
//                     new ROItemUpdateRequest
//                     {
//                         ProductVariantId = "11748",
//                         QuantityReceived = 10
//                     }
//                 }                    
//             }), Encoding.UTF8, "application/json");
//             // vtokenBearer = System.Environment.GetEnvironmentVariable("token", EnvironmentVariableTarget.User);
//             request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenBearer);
//
//             // Act
//             
//             var response = await _client.SendAsync(request);
//             response.EnsureSuccessStatusCode();
//             var value = await response.Content.ReadAsStringAsync();
//             
//             string id = value.Split(":")[1].Trim(new Char[]{'\"', '}'});
//             File.WriteAllText (@"/home/thomasm/InventoryManagementSystem/test/IMSBackend.Tests/UpdateGoodsReceipt.txt", value);
//             Assert.NotNull(id);
//         }
//         
//         [Fact]
//         private async Task UpdateProductItemGoodsReceipt()
//         {
//             var request = new HttpRequestMessage(HttpMethod.Put, "api/goodsreceipt/updateitem");
//             request.Content = new StringContent(JsonSerializer.Serialize(new
//             {
//                 productVariantId = "11748",
//                 sku = "fwoekfweopkop21",
//                 salePrice = 3123
//             }), Encoding.UTF8, "application/json");
//             var tokenBearer = System.Environment.GetEnvironmentVariable("token",  EnvironmentVariableTarget.User);
//
//             request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenBearer);
//
//             var response = await _client.SendAsync(request);
//             response.EnsureSuccessStatusCode();
//         }
//         
//         [Theory]
//         [InlineData("")]
//         private async Task SubmitGoodsReceipt(string receiptId)
//         {
//             var request = new HttpRequestMessage(HttpMethod.Post, "/api/goodsreceipt/submit");
//             request.Content = new StringContent(JsonSerializer.Serialize(new
//             {
//                 ReceivingOrderId = receiptId
//             }), Encoding.UTF8, "application/json");
//             request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenBearer);
//             
//             var response = await _client.SendAsync(request);
//             response.EnsureSuccessStatusCode();
//             var value = await response.Content.ReadAsStringAsync();
//         }
//         
//         [Fact]
//         private async Task GetGoodReceipt()
//         {
//             var request = new HttpRequestMessage(HttpMethod.Get, "/api/goodsreceipt/id/"+  System.Environment.GetEnvironmentVariable("receiptCreatedId",  EnvironmentVariableTarget.User));
//             // tokenBearer = System.Environment.GetEnvironmentVariable("token");
//             request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenBearer);
//                         
//             var response = await _client.SendAsync(request);
//             response.EnsureSuccessStatusCode();
//             var value = await response.Content.ReadAsStringAsync();
//             await File.WriteAllTextAsync("GetGoodReceipt.txt", value);
//             var data = (JObject)JsonConvert.DeserializeObject(value);
//             
//             Assert.NotEqual(data["supplier"], "null");
//             Assert.True(data["receivedOrderItems"].Count() > 0);
//         }
//     }
// }