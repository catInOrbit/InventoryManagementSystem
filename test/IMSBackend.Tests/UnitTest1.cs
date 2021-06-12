using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Infrastructure.Identity.DbContexts;
using Infrastructure.Services;
using InventoryManagementSystem.ApplicationCore.Entities.Orders;
using InventoryManagementSystem.ApplicationCore.Entities.Orders.Status;
using InventoryManagementSystem.ApplicationCore.Entities.RequestAndForm;
using InventoryManagementSystem.ApplicationCore.Interfaces;
using InventoryManagementSystem.PublicApi.GoodsIssueEndpoints.Create;
using InventoryManagementSystem.PublicApi.PurchaseOrderEndpoint.PurchaseOrder;
using InventoryManagementSystem.PublicApi.PurchaseOrderEndpoint.Search.PriceQuote;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Org.BouncyCastle.Asn1.Ocsp;
using Xunit;

namespace IMSBackend.Tests
{
    public class OperationalTest
    {
        public Mock<IAsyncRepository<PurchaseOrder>> purchaseOrderRepoMock = new Mock<IAsyncRepository<PurchaseOrder>>();

        private DbContextOptions<IdentityAndProductDbContext> dbContextOptions = new DbContextOptionsBuilder<IdentityAndProductDbContext>()
            .UseSqlServer("Server=localhost;Database=IMSDB3;User Id=SA;Password=Mcsoft 17 ubun;")
            .Options;
        public Mock<IAsyncRepository<PurchaseOrder>> PurchaseOrderMock = new Mock<IAsyncRepository<PurchaseOrder>>();

        [Fact]
        public async Task PurchaseOrderCreateTest()
        {
            var context = new IdentityAndProductDbContext(dbContextOptions);

            PurchaseOrder pqData = ((await context.PurchaseOrder.ToListAsync()).Where(pq => pq.PurchaseOrderProduct.Count > 0)).ToList()[0];
            var purchaseOrder = new PurchaseOrder();

            var transaction = new Transaction
            {
                CreatedDate = DateTime.Now,
                Type = TransactionType.Purchase,  
                CreatedById =  "Test"
            };

            purchaseOrder.Transaction = transaction;
            
            // purchaseOrder.PurchaseOrderStatus = PurchaseOrderStatusType.Created;
            if (pqData != null)
            {
                // purchaseOrder.PurchaseOrderNumber = pqData.PriceQuoteNumber;
                purchaseOrder.PurchaseOrderProduct = pqData.PurchaseOrderProduct;
                purchaseOrder.SupplierId = pqData.SupplierId;
            }
            
            Assert.True(purchaseOrder.PurchaseOrderProduct.Count > 0);
        }
                
    }

    
}
