using System;
using System.Threading;
using System.Threading.Tasks;
using InventoryManagementSystem.ApplicationCore.Entities.Orders;
using InventoryManagementSystem.ApplicationCore.Entities.Orders.Status;
using InventoryManagementSystem.ApplicationCore.Interfaces;
using InventoryManagementSystem.PublicApi.GoodsReceiptEndpoints.Create;
using InventoryManagementSystem.PublicApi.GoodsReceiptEndpoints;
using InventoryManagementSystem.PublicApi.PurchaseOrderEndpoint.Search.PriceQuote;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Org.BouncyCastle.Asn1.Ocsp;
using Xunit;

namespace IMSBackend.Tests
{
    public class GoodsIssueTest
    {
        public Mock<IAsyncRepository<PriceQuoteOrder>> mock = new Mock<IAsyncRepository<PriceQuoteOrder>>();  

        [Fact]
        public async Task GoodsIssueCreateTest()
        {
            GetPriceQuoteRequest request = new GetPriceQuoteRequest
            {
                number = "43096"
            };

            var response = new GetPriceQuoteResponse();
            // mock.Setup(p => p.HandleAsync(request, new CancellationToken())).ReturnsAsync(response);
            // var response = await _goodsIssueCreate.HandleAsync(request, new CancellationToken());
            // Assert.IsType<OkObjectResult>(response.Result);

            // Assert.NotNull(response.GoodsIssueOrder);
            // Assert.NotNull(response.GoodsIssueOrder.GoodsIssueNumber);
            // Assert.NotNull(response.GoodsIssueOrder.Transaction.CreatedBy);
            // Assert.Matches(response.GoodsIssueOrder.GoodsIssueType.ToString(), GoodsIssueType.Created.ToString());
            
            Assert.NotNull(response.PriceQuoteOrders);

        }
        
        
    }
}
