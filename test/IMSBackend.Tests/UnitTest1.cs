using System;
using System.Threading;
using System.Threading.Tasks;
using Infrastructure.Services;
using InventoryManagementSystem.ApplicationCore.Entities.Orders;
using InventoryManagementSystem.ApplicationCore.Entities.Orders.Status;
using InventoryManagementSystem.ApplicationCore.Interfaces;
using InventoryManagementSystem.PublicApi.GoodsIssueEndpoints.Create;
using InventoryManagementSystem.PublicApi.PurchaseOrderEndpoint.Search.PriceQuote;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Org.BouncyCastle.Asn1.Ocsp;
using Xunit;

namespace IMSBackend.Tests
{
    public class GoodsIssueTest
    {
        public Mock<IAsyncRepository<GoodsIssueOrder>> GoodIssueReposMock = new Mock<IAsyncRepository<GoodsIssueOrder>>();
        public Mock<IUserAuthentication> userAuthenticationMock = new Mock<IUserAuthentication>();
        public Mock<IAuthorizationService> authorizationMock = new Mock<IAuthorizationService>();   

        [Fact]
        public async Task GoodsIssueCreateTest()
        {
            GetPriceQuoteRequest request = new GetPriceQuoteRequest
            {
                number = "43096"
            };

            var response = new GetPriceQuoteResponse();

            var giCreate = new GoodsIssueCreate(userAuthenticationMock.Object, GoodIssueReposMock.Object, authorizationMock.Object);
            
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
