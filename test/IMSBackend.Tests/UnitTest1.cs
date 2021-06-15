// using System;
// using System.Linq;
// using System.Threading;
// using System.Threading.Tasks;
// using Infrastructure.Identity.DbContexts;
// using Infrastructure.Services;
// using InventoryManagementSystem.ApplicationCore.Entities.Orders;
// using InventoryManagementSystem.ApplicationCore.Entities.Orders.Status;
// using InventoryManagementSystem.ApplicationCore.Entities.SearchIndex;
// using InventoryManagementSystem.ApplicationCore.Interfaces;
// using InventoryManagementSystem.PublicApi.GoodsIssueEndpoints.Create;
// using InventoryManagementSystem.PublicApi.PurchaseOrderEndpoint.PurchaseOrder;
// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.EntityFrameworkCore;
// using Moq;
// using Org.BouncyCastle.Asn1.Ocsp;
// using Xunit;
//
// namespace IMSBackend.Tests
// {
//     public class OperationalTest
//     {
//         public Mock<IAsyncRepository<PurchaseOrder>> _purchaseOrderRepoMock = new Mock<IAsyncRepository<PurchaseOrder>>();
//         public Mock<IAsyncRepository<PurchaseOrderSearchIndex>> _indexAsyncRepositoryMock = new Mock<IAsyncRepository<PurchaseOrderSearchIndex>>();
//
//         private DbContextOptions<IdentityAndProductDbContext> dbContextOptions = new DbContextOptionsBuilder<IdentityAndProductDbContext>()
//             .UseSqlServer("Server=localhost;Database=IMSDB3;User Id=SA;Password=Mcsoft 17 ubun;")
//             .Options;
//         [Fact]
//         public async Task RequisitionCreateTest()
//         {
//             var context = new IdentityAndProductDbContext(dbContextOptions);
//             
//
//             _purchaseOrderRepoMock.Setup(po => po.AddAsync)
//             
//
//         }
//     }
//
//     
// }
