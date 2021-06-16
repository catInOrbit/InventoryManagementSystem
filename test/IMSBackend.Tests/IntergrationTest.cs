
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Policy;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using InventoryManagementSystem.ApplicationCore.Entities.Orders;
using InventoryManagementSystem.ApplicationCore.Interfaces;
using InventoryManagementSystem.PublicApi;
using InventoryManagementSystem.PublicApi.ReceivingOrderEndpoints;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Moq;
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

        private string tokenBearer;
        
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
            response.EnsureSuccessStatusCode();
            
            var value = await response.Content.ReadAsStringAsync();
            String[] seperator = {":", ","};
            var token = value.Split(seperator,7, StringSplitOptions.RemoveEmptyEntries);
            tokenBearer = token[5];
            tokenBearer =
                "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6ImM3ODY3NmY2LTc1NTUtNGU3ZS05OWQ5LWE4OTcxZGI4NWU5MiIsIm5iZiI6MTYyMzg2NTYzNCwiZXhwIjoxNjI0NDcwNDM0LCJpYXQiOjE2MjM4NjU2MzR9.U3LC_R02VBH4mcEDNtxbfSh3EWsQ1TH-NWL5qaafCyU";
            Console.WriteLine(tokenBearer);
            // _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6ImM3ODY3NmY2LTc1NTUtNGU3ZS05OWQ5LWE4OTcxZGI4NWU5MiIsIm5iZiI6MTYyMzg2NTA0MCwiZXhwIjoxNjI0NDY5ODQwLCJpYXQiOjE2MjM4NjUwNDB9.R4PdfhLqs4z-zOpMiCMXyD2xYm5UXNTPR_AdkkASLSw");        
        }

        [Fact]
        private async Task UpdateGoodsReceipt()
        {
            string body = File.ReadAllText(@"/home/thomasm/InventoryManagementSystem/test/IMSBackend.Tests/GoodsReceiptRequests/UpdateRequest");
            
            var request = new HttpRequestMessage(HttpMethod.Put, "api/goodsreceipt/update");
            request.Content = new StringContent(JsonSerializer.Serialize(new
            {
                purchaseOrderNumber = "PO6XW9M1Y4",
                updateItems = new List<ROItemUpdateRequest>
                {
                    new ROItemUpdateRequest
                    {
                        ProductVariantId = "11748",
                        QuantityReceived = 10
                    }
                }                    
            }), Encoding.UTF8, "application/json");
            tokenBearer =
                "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6ImM3ODY3NmY2LTc1NTUtNGU3ZS05OWQ5LWE4OTcxZGI4NWU5MiIsIm5iZiI6MTYyMzg2NjA3NSwiZXhwIjoxNjI0NDcwODc1LCJpYXQiOjE2MjM4NjYwNzV9.ZS1WfUKZNJRgaem4FpTYklOGU8I66cSgdJMLTSSTkxM";
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenBearer);

            // Act
            
            var response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var value = await response.Content.ReadAsStringAsync();
            
            string id = value.Split(":")[1].Trim(new Char[]{'\"', '}'});
            System.Environment.SetEnvironmentVariable("receiptCreatedId", id);
            Assert.NotNull(id);
        }
        
        [Fact]
        private async Task UpdateProductItemGoodsReceipt()
        {
            var request = new HttpRequestMessage(HttpMethod.Put, "api/goodsreceipt/updateitem");
            request.Content = new StringContent(JsonSerializer.Serialize(new
            {
                productVariantId = "11748",
                productStorageLocation = "Back",
                sku = "fwoekfweopkop21",
                salePrice = 3123
            }), Encoding.UTF8, "application/json");
            tokenBearer =
                "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6ImM3ODY3NmY2LTc1NTUtNGU3ZS05OWQ5LWE4OTcxZGI4NWU5MiIsIm5iZiI6MTYyMzg2NjA3NSwiZXhwIjoxNjI0NDcwODc1LCJpYXQiOjE2MjM4NjYwNzV9.ZS1WfUKZNJRgaem4FpTYklOGU8I66cSgdJMLTSSTkxM";
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenBearer);

            var response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();
        }
        
        [Fact]
        private async Task SubmitGoodsReceipt()
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "/api/goodsreceipt/submit");
            request.Content = new StringContent(JsonSerializer.Serialize(new
            {
                ReceivingOrderId =  System.Environment.GetEnvironmentVariable("receiptCreatedId")
            }), Encoding.UTF8, "application/json");
            tokenBearer =
                "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6ImM3ODY3NmY2LTc1NTUtNGU3ZS05OWQ5LWE4OTcxZGI4NWU5MiIsIm5iZiI6MTYyMzg2NjA3NSwiZXhwIjoxNjI0NDcwODc1LCJpYXQiOjE2MjM4NjYwNzV9.ZS1WfUKZNJRgaem4FpTYklOGU8I66cSgdJMLTSSTkxM";
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenBearer);
            
            var response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var value = await response.Content.ReadAsStringAsync();
        }
        
        [Theory]
        [InlineData("GRDE67BB78")]
        private async Task GetGoodReceipt(string id)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/goodsreceipt/id/"+id);
            tokenBearer =
                "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6ImM3ODY3NmY2LTc1NTUtNGU3ZS05OWQ5LWE4OTcxZGI4NWU5MiIsIm5iZiI6MTYyMzg2NjA3NSwiZXhwIjoxNjI0NDcwODc1LCJpYXQiOjE2MjM4NjYwNzV9.ZS1WfUKZNJRgaem4FpTYklOGU8I66cSgdJMLTSSTkxM";
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenBearer);
                        
            var response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var value = await response.Content.ReadAsStringAsync();
            
            GoodsReceiptOrder ro = JsonSerializer.Deserialize<GoodsReceiptOrder>(value);
            
            Assert.NotNull(ro.PurchaseOrderId);
            Assert.NotNull(ro.Transaction.CreatedBy);
            Assert.True(ro.ReceivedOrderItems.Count >=0);
        }
    }
}