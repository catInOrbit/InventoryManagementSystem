using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InventoryManagementSystem.ApplicationCore.Entities;
using InventoryManagementSystem.ApplicationCore.Entities.Orders;
using InventoryManagementSystem.ApplicationCore.Entities.Orders.Status;
using InventoryManagementSystem.ApplicationCore.Entities.Products;
using InventoryManagementSystem.ApplicationCore.Interfaces;

namespace InventoryManagementSystem.ApplicationCore.Services
{
    public class ReportBusinessService
    {
        public async Task<decimal> CalculateInventoryCostThisMonth(IAsyncRepository<Package> packageAsyncRepository)
        {
            var packages = (await packageAsyncRepository.ListAllAsync(new PagingOption<Package>(0, 0))).ResultList.ToList();
            packages = packages.Where(t => t.Transaction!= null && t.Transaction.TransactionRecord.Count > 0).ToList();
            packages = packages.Where(t=> t.Transaction.TransactionRecord[^1].Date.Month == DateTime.UtcNow.Month).ToList();
            decimal totalSum = 0;
            foreach (var package in packages)
            {
                totalSum += (package.Price * package.Quantity);
            }

            return totalSum;
        }
        
        public async Task<int> CalculateInventoryQuantityOfAllProducts(IAsyncRepository<Package> packageAsyncRepository)
        {
            var packages = (await packageAsyncRepository.ListAllAsync(new PagingOption<Package>(0, 0))).ResultList.ToList();
            packages = packages.Where(t => t.Transaction!= null && t.Transaction.TransactionRecord.Count > 0 ).ToList();
            packages = packages.Where(t=> t.Transaction.TransactionRecord[^1].Date.Month == DateTime.UtcNow.Month).ToList();
            int totalQuantity = 0;
            foreach (var package in packages)
            {
                totalQuantity +=  package.Quantity;
            }

            return totalQuantity;
        }
        
        public async Task<Dictionary<int,int>> CalculateImportExportQuantityThisMonth(IAsyncRepository<GoodsReceiptOrder> receiptsAsyncRepository,
            IAsyncRepository<GoodsIssueOrder> giAsyncRepository)
        {
            Dictionary<int, int> importAndExport = new Dictionary<int, int>();
            var goodsReceiptOrders = (await receiptsAsyncRepository.ListAllAsync(new PagingOption<GoodsReceiptOrder>(0, 0))).ResultList.ToList();
            goodsReceiptOrders = goodsReceiptOrders.Where(t =>t.Transaction!= null &&
                                                              t.Transaction.TransactionRecord.Count > 0 ).ToList();
            
            goodsReceiptOrders = goodsReceiptOrders.Where(t =>t.Transaction!= null &&
                                                              t.Transaction.TransactionRecord[^1].Date.Month == DateTime.UtcNow.Month).ToList();
            int totalImport = 0;
            
            foreach (var receiptOrder in goodsReceiptOrders)
            {
                foreach (var oi in receiptOrder.ReceivedOrderItems)
                {
                    totalImport += oi.QuantityReceived;
                }
            }
            
            var issues = (await giAsyncRepository.ListAllAsync(new PagingOption<GoodsIssueOrder>(0, 0))).ResultList.ToList();
            issues = issues.Where(t =>t.Transaction!= null &&
                                      t.Transaction.TransactionRecord.Count > 0
                ).ToList();
            
            foreach (var goodsIssueOrder in issues)
            {
                foreach (var transactionRecord in goodsIssueOrder.Transaction.TransactionRecord)
                {
                    Console.WriteLine(goodsIssueOrder.Id);
                    Console.WriteLine(transactionRecord.Date);
                }

            }
            issues = issues.Where(t =>
                t.Transaction.TransactionRecord[^1].Date.Month == DateTime.UtcNow.Month
                && t.GoodsIssueType == GoodsIssueStatusType.Completed
            ).ToList();
            int totalExport = 0;
            foreach (var issueOrder in issues)
            {
                foreach (var oi in issueOrder.GoodsIssueProducts)
                {
                    totalExport += oi.OrderQuantity;
                }
            }
            
            importAndExport.Add(totalImport, totalExport);
            return importAndExport;
        }

        // public async Task<Category> GetTopSellingCategoryThisMonth(IAsyncRepository<Product> productAsyncRepository,
        //     IAsyncRepository<ProductVariant> productVariantAsyncRepository,
        //     IAsyncRepository<Category> categoryAsyncRepository)
        // {
        //     var categories = (await categoryAsyncRepository.ListAllAsync(new PagingOption<Category>(0, 0))).ResultList
        //         .ToList();
        //
        //     Dictionary<string, int> categoryIDAndQuantityImport = new Dictionary<string, int>();
        //     
        //     foreach (var category in categories)
        //     {
        //         
        //     }
        // }
    }
}