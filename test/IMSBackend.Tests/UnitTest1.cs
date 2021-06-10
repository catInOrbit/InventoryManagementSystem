// using System;
// using System.Threading;
// using System.Threading.Tasks;
// using Infrastructure.Services;
// using InventoryManagementSystem.ApplicationCore.Entities.Orders;
// using InventoryManagementSystem.ApplicationCore.Entities.Orders.Status;
// using InventoryManagementSystem.ApplicationCore.Entities.RequestAndForm;
// using InventoryManagementSystem.ApplicationCore.Interfaces;
// using InventoryManagementSystem.PublicApi.GoodsIssueEndpoints.Create;
// using InventoryManagementSystem.PublicApi.PurchaseOrderEndpoint.PurchaseOrder;
// using InventoryManagementSystem.PublicApi.PurchaseOrderEndpoint.Search.PriceQuote;
// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Mvc;
// using Moq;
// using Org.BouncyCastle.Asn1.Ocsp;
// using Xunit;
//
// namespace IMSBackend.Tests
// {
//     public class OperationalTest
//     {
//         public Mock<IAsyncRepository<PurchaseOrder>> purchaseOrderRepoMock = new Mock<IAsyncRepository<PurchaseOrder>>();
//         public Mock<IAsyncRepository<PriceQuoteOrder>> priceQuoteRepoRepoMock = new Mock<IAsyncRepository<PriceQuoteOrder>>();
//
//         
//         public Mock<IAsyncRepository<PurchaseOrder>> PurchaseOrderMock = new Mock<IAsyncRepository<PurchaseOrder>>();
//
//         [Fact]
//         public async Task PurchaseOrderCreateTest()
//         {
//             var purchaseOrder = new PurchaseOrder();
//
//             var transaction = new Transaction
//             {
//                 CreatedDate = DateTime.Now,
//                 Type = TransactionType.Purchase,
//                 CreatedById = "Test"
//             };
//
//              purchaseOrder.Transaction = transaction;
//
//             POCreateRequest request = new POCreateRequest
//             {
//                 PriceQuoteNumber = "97313"
//             };
//
//             PriceQuoteOrder pqData = new PriceQuoteOrder();
//             priceQuoteRepoRepoMock.Setup(p => p.GetByIdAsync("03048", new CancellationToken())).ReturnsAsync(pqData);
//             // purchaseOrderRepoMock.Setup(pu => pu).Returns((IAsyncRepository<PurchaseOrder>) null);
//             PurchaseOrderCreate poc = new PurchaseOrderCreate(
//                 priceQuoteRepoRepoMock.Object);
//
//             var result =  await poc.HandleAsyncTest(request, pqData);
//         }
//                 
//     }
//
//     
// }
