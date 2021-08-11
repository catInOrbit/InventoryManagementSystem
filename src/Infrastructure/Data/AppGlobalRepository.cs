    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Ardalis.Specification;
    using Infrastructure.Identity.DbContexts;
    using InventoryManagementSystem.ApplicationCore.Entities;
    using InventoryManagementSystem.ApplicationCore.Entities.Orders;
    using InventoryManagementSystem.ApplicationCore.Entities.Orders.Status;
    using InventoryManagementSystem.ApplicationCore.Entities.Products;
    using InventoryManagementSystem.ApplicationCore.Entities.Reports;
    using InventoryManagementSystem.ApplicationCore.Extensions;
    using InventoryManagementSystem.ApplicationCore.Interfaces;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Nest;

    namespace Infrastructure.Data
{
    public class AppGlobalRepository<T> : IAsyncRepository<T> where T : BaseEntity
    {
        private readonly IdentityAndProductDbContext _identityAndProductDbContext;

        public AppGlobalRepository(IdentityAndProductDbContext identityAndProductDbContext)
        {
            _identityAndProductDbContext = identityAndProductDbContext;
        }

        public async Task<T> GetByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            var keyValues = new object[] { id };
            return await _identityAndProductDbContext.Set<T>().FindAsync(keyValues, cancellationToken);
        }

        public async Task<List<GoodsReceiptOrder>> GetAllGoodsReceiptsOfPurchaseOrder(string purchaseOrderId, CancellationToken cancellationToken = default)
        {
            return await _identityAndProductDbContext.Set<GoodsReceiptOrder>()
                .Where(gr => gr.PurchaseOrderId == purchaseOrderId).ToListAsync();
        }

        public async Task<PagingOption<Category>> GetCategory(PagingOption<Category> pagingOption, CancellationToken cancellationToken = default)
        {
            var listCategory = await _identityAndProductDbContext.Category.Where( ca => ca.Transaction.TransactionRecord.Count > 0 ).ToListAsync();
            pagingOption.ResultList = listCategory.OrderByDescending(ca =>
                ca.Transaction.TransactionRecord[^1].Date).ToList();
            pagingOption.ExecuteResourcePaging();
            return pagingOption;
        }

        public async Task<PagingOption<Location>> GetLocation(PagingOption<Location> pagingOption, CancellationToken cancellationToken = default)
        {
            var listLocations = await _identityAndProductDbContext.Location.Where( ca => ca.Transaction.TransactionRecord.Count > 0 ).ToListAsync();
            foreach (var listLocation in listLocations)
            {
                listLocation.LatestUpdateDate = listLocation.Transaction.TransactionRecord[^1].Date;
            }
            
            pagingOption.ResultList = listLocations.OrderByDescending(ca =>
                ca.Transaction.TransactionRecord[^1].Date).ToList();
            
            pagingOption.ExecuteResourcePaging();
            return pagingOption;
        }

        public async Task<List<Package>> GetPackagesFromProductVariantId(string productVariantId, CancellationToken cancellationToken = default)
        {
            return await _identityAndProductDbContext.Package.Where(package => package.ProductVariantId == productVariantId).OrderByDescending(package => package.ImportedDate)
                .ToListAsync(cancellationToken: cancellationToken);
        }

     
        public List<MergedOrderIdList> GetMergedPurchaseOrders(string parentOrderId, CancellationToken cancellationToken = default)
        {
            List<MergedOrderIdList> returnList = new List<MergedOrderIdList>();
            var result = _identityAndProductDbContext.PurchaseOrder
                .Where(o => o.MergedWithPurchaseOrderId == parentOrderId).ToList();
            
            foreach (var purchaseOrder in result)
            {
                returnList.Add( new MergedOrderIdList()
                {
                    ParentOrderId = parentOrderId,
                    PurchaseOrderId = purchaseOrder.Id,
                    CreatedDate = (purchaseOrder.Transaction.TransactionRecord.Count > 0 &&
                                   purchaseOrder.Transaction.TransactionRecord[0] != null)
                        ? purchaseOrder.Transaction.TransactionRecord
                            .OrderByDescending(e => e.UserTransactionActionType == UserTransactionActionType.Create).First()
                            .Date
                        : DateTime.MinValue,
                    CreatedBy = (purchaseOrder.Transaction.TransactionRecord.Count > 0 &&
                                 purchaseOrder.Transaction.TransactionRecord.FirstOrDefault(t =>
                                     t.UserTransactionActionType == UserTransactionActionType.Create) != null)
                        ? purchaseOrder.Transaction.TransactionRecord
                            .OrderByDescending(e => e.UserTransactionActionType == UserTransactionActionType.Create).First()
                            .ApplicationUser
                            .Fullname
                        : ""
                });
            }

            return returnList;
        }
      
        public GoodsIssueOrder GetGoodsIssueOrderByNumber(string goodsIssueOrderId, CancellationToken cancellationToken = default)
        {
            return _identityAndProductDbContext.GoodsIssueOrder.Where(go => go.Id == goodsIssueOrderId).
                SingleOrDefault(po => po.Id == goodsIssueOrderId);
        }
        
        public async Task<PagingOption<Package>> GetPackages(PagingOption<Package> pagingOption, CancellationToken cancellationToken = default)
        {
            pagingOption.ResultList = await _identityAndProductDbContext.Package.Where(t => t.Transaction.TransactionStatus!=false).OrderByDescending(pa => pa.ImportedDate).ToListAsync();
            pagingOption.ExecuteResourcePaging();
            return pagingOption;
        }
        
        public async Task<PagingOption<T>> ListAllAsync(PagingOption<T> pagingOption, CancellationToken cancellationToken = default)
        {
            pagingOption.ResultList = await _identityAndProductDbContext.Set<T>().ToListAsync(cancellationToken);
            pagingOption.ExecuteResourcePaging();
            return pagingOption;
        }

        public async Task<List<Notification>> ListAllNotificationByChannel(string channel, CancellationToken cancellationToken = default)
        {
            var notifications =
                await _identityAndProductDbContext.Notification.Where(n => n.Channel == channel).ToListAsync();

            return notifications;
        }

        public async Task<PagingOption<Supplier>> GetSuppliers(PagingOption<Supplier> pagingOption, CancellationToken cancellationToken = default)
        {
            var list = await _identityAndProductDbContext.Supplier.Where(s =>
                s.Transaction.TransactionRecord.Count > 0 && s.Transaction.TransactionStatus != false &&
                s.Transaction.CurrentType != TransactionType.Deleted).ToListAsync();

            list = list.OrderByDescending(s =>
                s.Transaction.TransactionRecord[s.Transaction.TransactionRecord.Count - 1].Date).ToList();
            foreach (var supplier in list)
            {
                if (supplier.Email == null) supplier.Email = "";
                if (supplier.Description == null) supplier.Description = "";
                if (supplier.SupplierName == null) supplier.SupplierName = "";
            }

            pagingOption.ResultList = list;
            pagingOption.ExecuteResourcePaging();
            return pagingOption;
        }

        public async Task<PagingOption<StockOnhandReport>> GenerateOnHandReport(PagingOption<StockOnhandReport> pagingOption, CancellationToken cancellationToken = default)
        {
            var productVariantNonNullList = await _identityAndProductDbContext.Set<ProductVariant>().
                Where(p => p.Packages.Count > 0).ToListAsync();
                
                
            productVariantNonNullList = productVariantNonNullList.OrderByDescending(pac => pac.Packages[pac.Packages.Count - 1].ImportedDate).ToList();
            
            // var list = await _identityAndProductDbContext.Set<Package>().Skip(pagingOption.SkipValue)
            //     .Take(pagingOption.SizePerPage).OrderByDescending(pac => pac.ImportedDate).ToListAsync();

            foreach (var productVariant in productVariantNonNullList)
            {
                try
                {
                    var packageList = await _identityAndProductDbContext.Set<Package>()
                        .Where(package => package.ProductVariantId == productVariant.Id).ToListAsync();

                    var stohReport = new StockOnhandReport();
                    stohReport.ProductVariantId = productVariant.Id;
                    stohReport.ProductVariantName = productVariant.Name;
                    if (packageList.Count == 0)
                    {
                        stohReport.DoesNotHavePackageInfo = true;
                        stohReport.CreatedDate = productVariant.Transaction.TransactionRecord[0].Date;
                        stohReport.StorageQuantity = productVariant.StorageQuantity;
                        stohReport.Value = productVariant.Price * productVariant.StorageQuantity;
                    }

                    else
                    {
                        foreach (var package in packageList)
                        {
                            stohReport.StockImportPackageInfos.Add(new StockImportInfo
                            {
                                Date = package.ImportedDate,
                                Value = package.TotalPrice,
                                StorageQuantity = package.Quantity
                            });
                        }
                    }
                    
                    pagingOption.ResultList.Add(stohReport);
                }
                catch (Exception e)
                {
                    Console.WriteLine(productVariant.Id);
                    throw;
                }
                
            }

            pagingOption.ExecuteResourcePaging(productVariantNonNullList.Count);
            return pagingOption;
        }

        public async Task<PagingOption<StockTakeReport>> GenerateStockTakeReport(PagingOption<StockTakeReport> pagingOption, CancellationToken cancellationToken = default)
        {
            var listNonNullTransac = await _identityAndProductDbContext.Set<StockTakeItem>()
                .Where(item => item.StockTakeOrder.Transaction.TransactionRecord.Count > 0).ToListAsync();
                
                
            listNonNullTransac = listNonNullTransac.OrderByDescending(item =>  item.StockTakeOrder.Transaction.TransactionRecord[item.StockTakeOrder.Transaction.TransactionRecord.Count - 1].Date).
                Skip(pagingOption.SkipValue).Take(pagingOption.SizePerPage).ToList();

            foreach (var stockTakeItem in listNonNullTransac)
            {
                var stockTakeReport = new StockTakeReport()
                {
                    StockTakeDate = stockTakeItem.StockTakeOrder.Transaction.TransactionRecord[0].Date,
                    ProductName = stockTakeItem.ProductVariantName,
                    StorageQuantity = stockTakeItem.StorageQuantity,
                    ActualQuantity = stockTakeItem.ActualQuantity,
                };
                
                pagingOption.ResultList.Add(stockTakeReport);
            }

            pagingOption.ExecuteResourcePaging(listNonNullTransac.Count);
            return pagingOption;
        }

        public async Task<PagingOption<TopSellingReport>> GenerateTopSellingReport(ReportType reportType, PagingOption<TopSellingReport> pagingOption, CancellationToken cancellationToken = default)
        {
            var nonNullTransaction = await _identityAndProductDbContext.Set<GoodsIssueOrder>()
                .Where(order => order.Transaction.TransactionRecord.Count > 0).ToListAsync();

            List<GoodsIssueOrder> timeFrameProductQuantity = null;

            switch (reportType)
            {
                case ReportType.Month:
                    timeFrameProductQuantity = nonNullTransaction.Where(order => 
                        order.Transaction.TransactionRecord[order.Transaction.TransactionRecord.Count - 1].Date.Month == DateTime.UtcNow.Month).ToList();
                    break;        
                
                case ReportType.Year:
                    timeFrameProductQuantity = nonNullTransaction.Where(order => 
                        order.Transaction.TransactionRecord[order.Transaction.TransactionRecord.Count - 1].Date.Year == DateTime.UtcNow.Year).ToList();
                    break;
            }
            
            
            foreach (var goodsIssueOrder in timeFrameProductQuantity)
            {
                var tempTop = goodsIssueOrder.GoodsIssueProducts.GroupBy(order => order.ProductVariant).Select(
                    g => new {ProductVariant = g.Key, OrderQuantityAggregrated = g.Sum(order => order.OrderQuantity)}
                );

                foreach (var x1 in tempTop)
                {
                    pagingOption.ResultList.Add(new TopSellingReport
                    {
                        ProductId = x1.ProductVariant.ProductId,
                        ProductName = x1.ProductVariant.Name,
                        TotalSold = x1.OrderQuantityAggregrated,
                        ReportType = reportType.ToString(),
                        ReportDate = $"{DateTime.UtcNow.Month}/{DateTime.UtcNow.Year}"
                    });
                }
            }

            pagingOption.ResultList = pagingOption.ResultList.OrderByDescending(x => x.TotalSold).ToList();
            
            // pagingOption.ResultList = await _identityAndProductDbContext.Set<ProductVariant>().Where(variant => variant.Packages[variant.Packages.Count-1].ImportedDate.Month == DateTime.Now.Month).
            //     OrderByDescending(item =>  item.StorageQuantity).Skip(pagingOption.SkipValue).Take(pagingOption.SizePerPage).ToListAsync();

            pagingOption.ExecuteResourcePaging();
            return pagingOption;
        }

        public Task<IReadOnlyList<T>> ListAsync(ISpecification<T> spec, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            await _identityAndProductDbContext.Set<T>().AddAsync(entity);
            await _identityAndProductDbContext.SaveChangesAsync(cancellationToken);

            return entity;
        }

        public async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
        {
            _identityAndProductDbContext.Entry(entity).State = EntityState.Modified;
            await _identityAndProductDbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
        {
            _identityAndProductDbContext.Set<T>().Remove(entity);
            await _identityAndProductDbContext.SaveChangesAsync(cancellationToken);
        }

        // public async Task DeletePurchaseOrderAsync(PurchaseOrder entity, CancellationToken cancellationToken = default)
        // {
        //     var po = _identityAndProductDbContext.PurchaseOrder.Where(po => po.Id == entity.Id);
        //     var poItems = _identityAndProductDbContext.OrderItem.Where(poItem => poItem.OrderId == entity.Id);
        //
        //     _identityAndProductDbContext.RemoveRange(poItems);
        //     _identityAndProductDbContext.Remove(po);
        //     await _identityAndProductDbContext.SaveChangesAsync(cancellationToken);
        // }

        public ApplicationUser GetUserInfoFromUserId(string userId)
        {
            return _identityAndProductDbContext.Users
                .FirstOrDefault(u => u.Id == userId);
        }
    }
}