    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Ardalis.Specification;
    using Infrastructure.Identity.DbContexts;
    using InventoryManagementSystem.ApplicationCore.Constants;
    using InventoryManagementSystem.ApplicationCore.Entities;
    using InventoryManagementSystem.ApplicationCore.Entities.Orders;
    using InventoryManagementSystem.ApplicationCore.Entities.Orders.Status;
    using InventoryManagementSystem.ApplicationCore.Entities.Products;
    using InventoryManagementSystem.ApplicationCore.Entities.Reports;
    using InventoryManagementSystem.ApplicationCore.Entities.SearchIndex;
    using InventoryManagementSystem.ApplicationCore.Interfaces;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Nest;

    namespace Infrastructure.Data
{
    public class AppGlobalRepository<T> : IAsyncRepository<T> where T : BaseEntity
    {
        private readonly IdentityAndProductDbContext _identityAndProductDbContext;
        private readonly IElasticClient _elasticClient;
        private readonly ILogger<AppGlobalRepository<T>> _logger;

        private List<T> _elasticCache = new List<T>();
        private List<ProductVariantSearchIndex> _elasticCacheProductSearchIndex = new List<ProductVariantSearchIndex>();

        public AppGlobalRepository(IdentityAndProductDbContext identityAndProductDbContext, IElasticClient elasticClient, ILogger<AppGlobalRepository<T>> logger)
        {
            _identityAndProductDbContext = identityAndProductDbContext;
            _elasticClient = elasticClient;
            _logger = logger;
        }

        public async Task<T> GetByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            var keyValues = new object[] { id };
            return await _identityAndProductDbContext.Set<T>().FindAsync(keyValues, cancellationToken);
        }
        
        public async Task<PagingOption<ProductSearchIndex>> GetProductForELIndexAsync(PagingOption<ProductSearchIndex> pagingOption, CancellationToken cancellationToken = default)
        {

            var products = await _identityAndProductDbContext.Product.Where(product =>
                product.Transaction.TransactionRecord.Count > 0 && product.Transaction.TransactionStatus != false &&
                product.Transaction.Type != TransactionType.Deleted).ToListAsync();
            

            products = products.OrderByDescending(e =>
                e.Transaction.TransactionRecord[e.Transaction.TransactionRecord.Count - 1].Date).ToList();
            
            foreach (var product in products)
            {
              
                try
                {
                    var index = IndexingHelper.ProductSearchIndex(product);
                    index.FillSuggestion();
                    pagingOption.ResultList.Add(index);
                }
                catch (Exception e)
                {
                    Console.WriteLine(product.Id);
                    Console.WriteLine(e);
                    throw;
                }
            }            
            
            pagingOption.ExecuteResourcePaging();
            return pagingOption;
        }

        public async Task<PagingOption<ProductVariantSearchIndex>> GetProductVariantForELIndexAsync(PagingOption<ProductVariantSearchIndex> pagingOption, CancellationToken cancellationToken = default)
        {
            
            var variants =  await _identityAndProductDbContext.ProductVariant.
                Where(variant => variant.Transaction.TransactionRecord.Count > 0 && variant.Transaction.TransactionStatus!=false && variant.Transaction.Type!=TransactionType.Deleted).ToListAsync(cancellationToken);
            variants = variants.OrderByDescending(e =>
                e.Transaction.TransactionRecord[e.Transaction.TransactionRecord.Count - 1].Date).ToList();
            foreach (var productVariant in variants)
            {
                try
                {
                    var index = IndexingHelper.ProductVariantSearchIndex(productVariant);
                    index.FillSuggestion();
                    pagingOption.ResultList.Add(index);
                }
                catch (Exception e)
                {
                    Console.WriteLine(productVariant.Id);
                    Console.WriteLine(e);
                    throw;
                }
            }            
            pagingOption.ExecuteResourcePaging();
            return pagingOption;
        }
        
        public async Task<PagingOption<PurchaseOrderSearchIndex>> GetPOForELIndexAsync(bool hideMergeStatus, PagingOption<PurchaseOrderSearchIndex> pagingOption, POSearchFilter poSearchFilter,  CancellationToken cancellationToken = default)
        {
            List<PurchaseOrder> pos;

            if(hideMergeStatus)
                pos = await _identityAndProductDbContext.PurchaseOrder.
                    Where(variant => variant.Transaction.TransactionRecord.Count > 0 && variant.Transaction.TransactionStatus!=false 
                        && variant.Transaction.Type!=TransactionType.Deleted
                        && variant.PurchaseOrderStatus != PurchaseOrderStatusType.RequisitionMerged
                        ).ToListAsync();
            else
                pos = await _identityAndProductDbContext.PurchaseOrder.
                    Where(variant => variant.Transaction.TransactionRecord.Count > 0 && variant.Transaction.TransactionStatus!=false 
                        && variant.Transaction.Type!=TransactionType.Deleted
                    ).ToListAsync();


            // pos = pos.OrderByDescending(e =>
            //     e.Transaction.TransactionRecord.OrderByDescending(r => r.Date).First().Date).ToList();
            

            // var listTransctionRecord = _identityAndProductDbContext.TransactionRecord
            //     .Where(t => t.UserTransactionActionType == UserTransactionActionType.Submit).ToList();
            //
            //
            // foreach (var record in listTransctionRecord)
            //     Console.WriteLine(record.Id);
            //
            // var matchList = from po in pos
            //     from record in listTransctionRecord
            //     where
            //         po.Transaction.TransactionRecord.Contains(record)
            //     select po;
          
            
            foreach (var po in pos)
            {
                PurchaseOrderSearchIndex index; 
                try
                {
                    pagingOption.ResultList.Add(IndexingHelper.PurchaseOrderSearchIndex(po));
                }
                catch (Exception e)
                {
                    Console.WriteLine(po.Id);
                    throw;
                }
            }
            pagingOption.ResultList = PurchaseOrderIndexFiltering(pagingOption.ResultList.ToList(), poSearchFilter);
            
            pagingOption.ExecuteResourcePaging();
            return pagingOption;
        }



        public async Task<PagingOption<Category>> GetCategory(PagingOption<Category> pagingOption, CancellationToken cancellationToken = default)
        {
            var listCategory = await _identityAndProductDbContext.Category.Where( ca => ca.Transaction.TransactionRecord.Count > 0 ).ToListAsync();
            pagingOption.ResultList = listCategory.OrderByDescending(ca =>
                ca.Transaction.TransactionRecord[ca.Transaction.TransactionRecord.Count - 1].Date).ToList();
            pagingOption.ExecuteResourcePaging();
            return pagingOption;
        }

        public async Task<List<Package>> GetPackagesFromProductVariantId(string productVariantId, CancellationToken cancellationToken = default)
        {
            return await _identityAndProductDbContext.Package.Where(package => package.ProductVariantId == productVariantId).OrderByDescending(package => package.ImportedDate)
                .ToListAsync(cancellationToken: cancellationToken);
        }

        public List<PurchaseOrderSearchIndex> PurchaseOrderIndexFiltering(List<PurchaseOrderSearchIndex> resource, POSearchFilter poSearchFilter, CancellationToken cancellationToken  = default)
        {
            var pos = resource.Where(po =>
                    ( (poSearchFilter.Statuses == null ||
                       poSearchFilter.Statuses.Contains((ParseEnum<PurchaseOrderStatusType>(po.Status).ToString()) ))
                      
                    &&
                    (poSearchFilter.FromDeliveryDate == null ||
                     (po.DeliveryDate >= DateTime.Parse(poSearchFilter.FromDeliveryDate) &&
                      po.DeliveryDate <= DateTime.Parse(poSearchFilter.ToDeliveryDate))) 
                    
                    &&
                    (poSearchFilter.FromCreatedDate == null ||
                     (po.CreatedDate >= DateTime.Parse(poSearchFilter.FromCreatedDate) &&
                      po.CreatedDate <= DateTime.Parse(poSearchFilter.ToCreatedDate))) 
                    
                    &&
                    (poSearchFilter.FromTotalOrderPrice == null ||
                     (po.TotalPrice >= Decimal.Parse(poSearchFilter.FromTotalOrderPrice) &&
                      po.TotalPrice <= Decimal.Parse(poSearchFilter.ToTotalOrderPrice))) 
                    
                    &&
                    (poSearchFilter.SupplierId == null || po.SupplierId == poSearchFilter.SupplierId) 
                    
                    &&
                    (poSearchFilter.CreatedByName == null || po.CreatedByName == poSearchFilter.CreatedByName)
                    &&
                    (poSearchFilter.FromModifiedDate == null ||
                     (po.ModifiedDate >= DateTime.Parse(poSearchFilter.FromModifiedDate) &&
                      po.ModifiedDate <= DateTime.Parse(poSearchFilter.ToModifiedDate)))
                    &&
                    (poSearchFilter.FromConfirmedDate == null ||
                     (po.ConfirmedDate >= DateTime.Parse(poSearchFilter.FromConfirmedDate) &&
                      po.ConfirmedDate <= DateTime.Parse(poSearchFilter.ToConfirmedDate)))
                    &&
                    (poSearchFilter.ConfirmedByName == null || po.ConfirmedByName == poSearchFilter.ConfirmedByName)
                    &&
                    (poSearchFilter.IgnoreOrderIds == null || !poSearchFilter.IgnoreOrderIds.Contains(po.Id)
                    
                )))
                .ToList();
            return pos;
        }

        

        private static T ParseEnum<T>(string value)
        {
            return (T) Enum.Parse(typeof(T), value, true);
        }

        public List<GoodsReceiptOrderSearchIndex> ReceivingOrderIndexFiltering(List<GoodsReceiptOrderSearchIndex> resource, ROSearchFilter roSearchFilter, CancellationToken cancellationToken = default)
        {
            var ros = resource.Where(ro =>
                    
                    (roSearchFilter.CreatedByName == null || ro.CreatedBy == roSearchFilter.CreatedByName)
                    &&
                    (roSearchFilter.FromCreatedDate == null ||
                     (ro.CreatedDate >= DateTime.Parse(roSearchFilter.FromCreatedDate) &&
                      ro.CreatedDate <= DateTime.Parse(roSearchFilter.ToCreatedDate))) 
                    &&
                    (roSearchFilter.SupplierName == null || ro.SupplierName == roSearchFilter.SupplierName)
                )
                .ToList();
            return ros;
        }

        public List<GoodsIssueSearchIndex> GoodsIssueIndexFiltering(List<GoodsIssueSearchIndex> resource, GISearchFilter giSearchFilter, CancellationToken cancellationToken =default)
        {
            var ros = resource.Where(gi =>
                    
                    (giSearchFilter.FromStatus  == null ||
                     (int)(ParseEnum<GoodsIssueStatusType>(gi.Status))  >= Int32.Parse(giSearchFilter.FromStatus)
                     &&
                     (int)(ParseEnum<GoodsIssueStatusType>(gi.Status))  <= Int32.Parse(giSearchFilter.ToStatus))
                    &&
                    (giSearchFilter.FromCreatedDate == null ||
                     (gi.CreatedDate >= DateTime.Parse(giSearchFilter.FromCreatedDate) &&
                      gi.CreatedDate <= DateTime.Parse(giSearchFilter.ToCreatedDate))) 
                    &&
                    (giSearchFilter.CreatedByName == null || gi.CreatedByName == giSearchFilter.CreatedByName)
                    &&
                    (giSearchFilter.DeliveryMethod == null || gi.DeliveryMethod == giSearchFilter.DeliveryMethod)
                    &&
                    (giSearchFilter.FromDeliveryDate == null ||
                     (gi.DeliveryDate >= DateTime.Parse(giSearchFilter.FromDeliveryDate) &&
                      gi.DeliveryDate <= DateTime.Parse(giSearchFilter.ToDeliveryDate))) 
                )
                .ToList();
            return ros;
        }

        public List<ProductVariantSearchIndex> ProductVariantIndexFiltering(List<ProductVariantSearchIndex> resource, ProductVariantSearchFilter productSearchFilter, CancellationToken cancellationToken)
        {
            var pos = resource.Where(product =>
                ( 
                    (productSearchFilter.FromCreatedDate == null ||
                     (product.CreatedDate >= DateTime.Parse(productSearchFilter.FromCreatedDate) &&
                      product.CreatedDate <= DateTime.Parse(productSearchFilter.ToCreatedDate))) 
                    
                    &&
                    (productSearchFilter.FromModifiedDate == null ||
                     (product.ModifiedDate >= DateTime.Parse(productSearchFilter.FromModifiedDate) &&
                      product.ModifiedDate <= DateTime.Parse(productSearchFilter.ToModifiedDate))) 
                    
                    &&
                    (productSearchFilter.Category == null ||
                     (product.Category == productSearchFilter.Category) 
                    
                     &&
                     (productSearchFilter.Strategy == null ||
                      (product.Strategy == productSearchFilter.Strategy) 
                    
                      &&
                      (productSearchFilter.CreatedByName == null || product.CreatedByName == productSearchFilter.CreatedByName)
                     
                      &&
                      (productSearchFilter.ModifiedByName == null || product.ModifiedByName == productSearchFilter.ModifiedByName)
                     
                     
                      &&
                      (productSearchFilter.FromPrice == null || 
                       (product.Price >= Decimal.Parse(productSearchFilter.FromPrice)
                        && product.Price <= Decimal.Parse(productSearchFilter.ToPrice)
                       ) )
                      &&
                      (productSearchFilter.Brand == null || product.Brand == productSearchFilter.Brand)
                     ))))
                .ToList();
            return pos;
        }

        public List<Package> PackageIndexFiltering(List<Package> resource, PackageSearchFilter packageSearchFilter,
            CancellationToken cancellationToken)
        {
            foreach (var package in resource)
            {
                if(package.Location == null)
                    Console.WriteLine(package.Id);
                if(package.ProductVariant == null)
                    Console.WriteLine(package.Id);
                if(package.ProductVariantId == null)
                    Console.WriteLine(package.Id);
            }
            
            var packages = resource.Where(package =>
                ( 
                    package.Location!=null 
                    &&
                    ((packageSearchFilter.FromImportedDate == null)  ||
                     (package.ImportedDate >= DateTime.Parse(packageSearchFilter.FromImportedDate) &&
                      package.ImportedDate <= DateTime.Parse(packageSearchFilter.ToImportedDate))) 
                    
                    &&
                    (packageSearchFilter.FromPrice == null ||
                     (package.Price >= Decimal.Parse(packageSearchFilter.FromPrice) &&
                      package.Price <= Decimal.Parse(packageSearchFilter.ToPrice))) 
                    
                    &&
                    (packageSearchFilter.FromTotalPrice == null ||
                     (package.TotalPrice >= Decimal.Parse(packageSearchFilter.FromTotalPrice) &&
                      package.TotalPrice <= Decimal.Parse(packageSearchFilter.ToTotalPrice))) 
                    
                    &&
                    (packageSearchFilter.FromQuantity == null ||
                     (package.Quantity >= int.Parse(packageSearchFilter.FromQuantity) &&
                      package.Quantity <= int.Parse(packageSearchFilter.ToQuantity))) 
                    
                    &&
                    (packageSearchFilter.LocationId == null  ||
                     (package.Location.Id == packageSearchFilter.LocationId) 
                    
                     &&
                     ((packageSearchFilter.ProductVariantID == null ) ||
                      (package.ProductVariantId == packageSearchFilter.ProductVariantID) 
        
                     ))))
                .ToList();
            return packages;
        }

        public List<ProductSearchIndex> ProductIndexFiltering(List<ProductSearchIndex> resource, ProductSearchFilter productSearchFilter, CancellationToken cancellationToken)
        {
            var pos = resource.Where(product =>
                ( 
                    (productSearchFilter.FromCreatedDate == null ||
                     (product.CreatedDate >= DateTime.Parse(productSearchFilter.FromCreatedDate) &&
                      product.CreatedDate <= DateTime.Parse(productSearchFilter.ToCreatedDate))) 
                    &&
                    (productSearchFilter.FromModifiedDate == null ||
                     (product.ModifiedDate >= DateTime.Parse(productSearchFilter.FromModifiedDate) &&
                      product.ModifiedDate <= DateTime.Parse(productSearchFilter.ToModifiedDate))) 
                    &&
                    (productSearchFilter.Category == null ||
                     (product.Category == productSearchFilter.Category) 
                    
                     &&
                     (productSearchFilter.Strategy == null ||
                      (product.Strategy == productSearchFilter.Strategy) 
                    
                      &&
                      (productSearchFilter.CreatedByName == null || product.CreatedByName == productSearchFilter.CreatedByName)
                     
                      &&
                      (productSearchFilter.ModifiedByName == null || product.ModifiedByName == productSearchFilter.ModifiedByName)
                      &&
                      (productSearchFilter.Brand == null || product.Brand == productSearchFilter.Brand)
                     ))))
                .ToList();
            return pos;
        }
        

        public List<StockTakeSearchIndex> StockTakeIndexFiltering(List<StockTakeSearchIndex> resource, STSearchFilter stSearchFilter, CancellationToken cancellationToken = default)
        {
            var sts = resource.Where(st =>
                ( 
                    (stSearchFilter.FromStatus  == null ||
                     (int)(ParseEnum<StockTakeOrderType>(st.Status))  >= Int32.Parse(stSearchFilter.FromStatus)
                     &&
                     (int)(ParseEnum<StockTakeOrderType>(st.Status))  <= Int32.Parse(stSearchFilter.ToStatus))
                    
                    &&
                    (stSearchFilter.FromCreatedDate == null ||
                     (st.CreatedDate >= DateTime.Parse(stSearchFilter.FromCreatedDate) &&
                      st.CreatedDate <= DateTime.Parse(stSearchFilter.ToCreatedDate))) 
                    
                    &&
                    (stSearchFilter.FromDeliveryDate == null ||
                     (st.ModifiedDate >= DateTime.Parse(stSearchFilter.FromDeliveryDate) &&
                      st.ModifiedDate <= DateTime.Parse(stSearchFilter.ToDeliveryDate))) 
                    
               
                      &&
                      (stSearchFilter.CreatedByName == null || st.CreatedByName == stSearchFilter.CreatedByName)
                     ))
                .ToList();
            return sts;
        }


        public string VariantNameConcat(List<string> productVariantValues)
        {
            string nameConcat = "";
            foreach (var productVariantValue in productVariantValues)
            {
                nameConcat += productVariantValue;
            }
            
            return nameConcat;
        }

        public async Task<PagingOption<GoodsReceiptOrderSearchIndex>> GetROForELIndexAsync(PagingOption<GoodsReceiptOrderSearchIndex> pagingOption, ROSearchFilter roSearchFilter, CancellationToken cancellationToken = default)
        {
            
            var ros = await _identityAndProductDbContext.Set<GoodsReceiptOrder>().
                Where(variant => variant.Transaction.TransactionRecord.Count > 0 &&variant.Transaction.TransactionStatus!=false && variant.Transaction.Type!=TransactionType.Deleted).ToListAsync(cancellationToken);
            ros = ros.OrderByDescending(e =>
                e.Transaction.TransactionRecord[e.Transaction.TransactionRecord.Count - 1].Date).ToList();

            foreach (var ro in ros)
            {
                try
                {
                    pagingOption.ResultList.Add(IndexingHelper.GoodsReceiptOrderSearchIndex(ro));
                }
                catch (Exception e)
                {
                    Console.WriteLine(ro.Id);
                    throw;
                }
            }

            pagingOption.ResultList = ReceivingOrderIndexFiltering(pagingOption.ResultList.ToList(), roSearchFilter);
            pagingOption.ExecuteResourcePaging();
            return pagingOption;
        }

        public async Task<PagingOption<GoodsIssueSearchIndex>> GetGIForELIndexAsync(PagingOption<GoodsIssueSearchIndex> pagingOption,GISearchFilter searchFilter, CancellationToken cancellationToken = default)
        {
            var gis = await _identityAndProductDbContext.Set<GoodsIssueOrder>().
                Where(variant => variant.Transaction.TransactionRecord.Count > 0 &&variant.Transaction.TransactionStatus!=false && variant.Transaction.Type!=TransactionType.Deleted).
                ToListAsync(cancellationToken);
            gis = gis.OrderByDescending(e =>
                e.Transaction.TransactionRecord[e.Transaction.TransactionRecord.Count - 1].Date).ToList();
            foreach (var gi in gis)
            {
                try
                {
                    pagingOption.ResultList.Add(IndexingHelper.GoodsIssueSearchIndexHelper(gi));
                }
                catch (Exception e)
                {
                    Console.WriteLine(gi.Id);
                    throw;
                }
            }
            
            pagingOption.ResultList = GoodsIssueIndexFiltering(pagingOption.ResultList.ToList(), searchFilter);
            pagingOption.ExecuteResourcePaging();
            return pagingOption;
        }

        public async Task<PagingOption<StockTakeSearchIndex>> GetSTForELIndexAsync(PagingOption<StockTakeSearchIndex> pagingOption,STSearchFilter stSearchFilter, CancellationToken cancellationToken = default)
        {
            
            var sts = await _identityAndProductDbContext.Set<StockTakeOrder>().
                Where(variant =>variant.Transaction.TransactionRecord.Count > 0 && variant.Transaction.TransactionStatus!=false && variant.Transaction.Type!=TransactionType.Deleted).ToListAsync(cancellationToken);
            
            sts = sts.OrderByDescending(e =>
                e.Transaction.TransactionRecord[e.Transaction.TransactionRecord.Count - 1].Date).ToList();
            
            
            foreach (var st in sts)
            {
                StockTakeSearchIndex index; 
                try
                {
                    index = new StockTakeSearchIndex
                    {
                        Id = st.Id,
                        CreatedByName = (st.Transaction.TransactionRecord.Count > 0) ? st.Transaction.TransactionRecord[^1].ApplicationUser.Fullname : "",
                        Status = st.StockTakeOrderType.ToString(),
                        CreatedDate = st.Transaction.TransactionRecord[0].Date,
                        ModifiedDate = st.Transaction.TransactionRecord[^1].Date
                    };
                    
                    pagingOption.ResultList.Add(index);
                }
                catch (Exception e)
                {
                    Console.WriteLine(st.Id);
                    Console.WriteLine(e);
                    throw;
                }
            }
            pagingOption.ResultList = StockTakeIndexFiltering(pagingOption.ResultList.ToList(), stSearchFilter);
            pagingOption.ExecuteResourcePaging();
            return pagingOption;
        }

        // public PriceQuoteOrder GetPriceQuoteByNumber(string priceQuoteNumber, CancellationToken cancellationToken = default)
        // {
        //     return _identityAndProductDbContext.PriceQuote.Where(pq => pq.PriceQuoteNumber == priceQuoteNumber).
        //         SingleOrDefault(pq => pq.PriceQuoteNumber == priceQuoteNumber);
        // }
        //
        public PurchaseOrder GetPurchaseOrderByNumber(string purchaseOrderId, CancellationToken cancellationToken = default)
        {
            return _identityAndProductDbContext.PurchaseOrder.Where(po => po.Id == purchaseOrderId && po.Transaction.TransactionStatus == true).
                SingleOrDefault(po => po.Id == purchaseOrderId);   
        }

        public GoodsReceiptOrder GetReceivingOrderByNumber(string receiveOrderId, CancellationToken cancellationToken = default)
        {
            return _identityAndProductDbContext.GoodsReceiptOrder.Where(go => go.Id == receiveOrderId).
                SingleOrDefault(po => po.Id == receiveOrderId);   
        }

        public GoodsIssueOrder GetGoodsIssueOrderByNumber(string goodsIssueOrderId, CancellationToken cancellationToken = default)
        {
            return _identityAndProductDbContext.GoodsIssueOrder.Where(go => go.Id == goodsIssueOrderId).
                SingleOrDefault(po => po.Id == goodsIssueOrderId);
        }
        
        public async Task<PagingOption<Package>> GetPackages(PagingOption<Package> pagingOption, CancellationToken cancellationToken = default)
        {
            pagingOption.ResultList = await _identityAndProductDbContext.Package.OrderByDescending(pa => pa.ImportedDate).ToListAsync();
            pagingOption.ExecuteResourcePaging();
            return pagingOption;
        }
        
        public async Task<PagingOption<T>> ListAllAsync(PagingOption<T> pagingOption, CancellationToken cancellationToken = default)
        {
            pagingOption.ResultList = await _identityAndProductDbContext.Set<T>().ToListAsync(cancellationToken);
            pagingOption.ExecuteResourcePaging();
            return pagingOption;
        }

        public async Task<PagingOption<Supplier>> GetSuppliers(PagingOption<Supplier> pagingOption, CancellationToken cancellationToken = default)
        {
            var list = await _identityAndProductDbContext.Supplier.Where(s =>
                s.Transaction.TransactionRecord.Count > 0 && s.Transaction.TransactionStatus != false &&
                s.Transaction.Type != TransactionType.Deleted).ToListAsync();

            list = list.OrderByDescending(s =>
                s.Transaction.TransactionRecord[s.Transaction.TransactionRecord.Count - 1].Date).ToList();

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
                    ProductName = stockTakeItem.Package.ProductVariant.Name,
                    StorageQuantity = stockTakeItem.Package.ProductVariant.StorageQuantity,
                    ActualQuantity = stockTakeItem.ActualQuantity,
                    Value = stockTakeItem.Package.ProductVariant.StorageQuantity * stockTakeItem.Package.ProductVariant.Price
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

            List<GoodsIssueOrder> thisMonthSales = null;

            switch (reportType)
            {
                case ReportType.Month:
                    thisMonthSales = nonNullTransaction.Where(order => 
                        order.Transaction.TransactionRecord[order.Transaction.TransactionRecord.Count - 1].Date.Month == DateTime.UtcNow.Month).ToList();
                    break;        
                
                case ReportType.Year:
                    thisMonthSales = nonNullTransaction.Where(order => 
                        order.Transaction.TransactionRecord[order.Transaction.TransactionRecord.Count - 1].Date.Year == DateTime.UtcNow.Year).ToList();
                    break;
            }
            
            
            Dictionary<ProductVariant, int> top = new Dictionary<ProductVariant, int>();
                
            foreach (var goodsIssueOrder in thisMonthSales)
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
                        ReportType = "Month",
                        ReportDate = $"{DateTime.UtcNow.Month}/{DateTime.UtcNow.Year}"
                    });
                }
            }
            
            // pagingOption.ResultList = await _identityAndProductDbContext.Set<ProductVariant>().Where(variant => variant.Packages[variant.Packages.Count-1].ImportedDate.Month == DateTime.Now.Month).
            //     OrderByDescending(item =>  item.StorageQuantity).Skip(pagingOption.SkipValue).Take(pagingOption.SizePerPage).ToListAsync();

            pagingOption.ExecuteResourcePaging();
            return pagingOption;
        }

        public async Task<IEnumerable<Product>> ListAllProductAsync(CancellationToken cancellationToken = default)
        {
            var query =  await _identityAndProductDbContext.Set<Product>()
                .Join(
                    _identityAndProductDbContext.ProductVariant,
                    product => product.Id,
                    variant => variant.ProductId,
                    (entryPoint, entry) => new {entryPoint, entry}
                ).ToListAsync(cancellationToken);
            return (IEnumerable<Product>) query;
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

        public async Task DeletePurchaseOrderAsync(PurchaseOrder entity, CancellationToken cancellationToken = default)
        {
            var po = _identityAndProductDbContext.PurchaseOrder.Where(po => po.Id == entity.Id);
            var poItems = _identityAndProductDbContext.OrderItem.Where(poItem => poItem.OrderId == entity.Id);

            _identityAndProductDbContext.RemoveRange(poItems);
            _identityAndProductDbContext.Remove(po);
            await _identityAndProductDbContext.SaveChangesAsync(cancellationToken);
        }

        public Task<int> CountAsync(ISpecification<T> spec, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<T> FirstAsync(ISpecification<T> spec, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<T> FirstOrDefaultAsync(ISpecification<T> spec, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
        
        public async Task ElasticSaveSingleAsync(bool isSavingNew, T type, string index)
        {

            try
            {
                await ElasticDeleteSingleAsync(type, index);
            }
            catch
            {
                
            }
            // Console.WriteLine("ElasticSaveSingleAsync: Type: " + type.GetType() + " || Reindexing");
            _logger.LogInformation("ElasticSaveSingleAsync: Type: " + type.GetType() + " || Reindexing");
            var response = await _elasticClient.IndexAsync(type, i => i.Index(index));
            if (!response.IsValid)
                throw new Exception(response.DebugInformation);

            // if (!isSavingNew)
            // {
            //     Console.WriteLine("ElasticSaveSingleAsync: Type: " + type.GetType() + " || Update");
            //    // var response = await _elasticClient.UpdateAsync<T>(type, u => u.Index(index).Doc(type));
            //     var response = await _elasticClient.UpdateAsync<T>(type, u => u.Index(index).Doc(type));
            //
            //     if (!response.IsValid)
            //         throw new Exception(response.DebugInformation);
            // }
            //
            // else
            // {
            //     Console.WriteLine("ElasticSaveSingleAsync: Type: " + type.GetType() + " || AddNew");
            //     // _elasticCache.Add(type);
            //     // await _elasticClient.IndexDocumentAsync<T>(type);
            //     var response = await _elasticClient.IndexAsync(type, i => i.Index(index));
            //     if (!response.IsValid)
            //         throw new Exception(response.DebugInformation);
            // }
        }

        public async Task ElasticSaveManyAsync(T[] types)
        {
            // await _elasticClient.DeleteByQueryAsync<T>(q => q.MatchAll());
            _elasticCache.AddRange(types);
            var result = await _elasticClient.IndexManyAsync(types);
            if (result.Errors)
            {
                // the response can be inspected for errors
                foreach (var itemWithError in result.ItemsWithErrors)
                {
                    Console.WriteLine(itemWithError.Error);
                    throw new Exception();
                }
            }
        }
        
        public async Task ReIndexProduct()
        {
            await _elasticClient.Indices.DeleteAsync(ElasticIndexConstant.PRODUCT_INDICES);
            await _elasticClient.Indices.CreateAsync( ElasticIndexConstant.PRODUCT_INDICES,
                index 
                    => index.Map<ProductSearchIndex>(x 
                        => x.AutoMap().Properties(ps 
                            => ps.Completion(c 
                                => c.Name(n => n.Suggest))))
            );
            
            // await ElasticSaveBulkAsync((await GetProductVariantForELIndexAsync(new PagingOption<ProductVariantSearchIndex>(0, 0))).ResultList.ToArray(),  ElasticIndexConstant.PRODUCT_VARIANT_INDICES);
            // await ElasticSaveBulkAsync((await GetProductForELIndexAsync(new PagingOption<ProductSearchIndex>(0, 0))).ResultList.ToArray(),  ElasticIndexConstant.PRODUCT_INDICES);
        }


        public async Task ReIndexProductVariant()
        {
            await _elasticClient.Indices.DeleteAsync(ElasticIndexConstant.PRODUCT_VARIANT_INDICES);

            await _elasticClient.Indices.CreateAsync( ElasticIndexConstant.PRODUCT_VARIANT_INDICES,
                index 
                    => index.Settings(s=> s.Analysis(
                            a => a.Tokenizers(
                                t => t.Pattern("hyphen_tokenizer", descriptor => descriptor.Pattern(".*-.*"))
                            )
                        ))
                        .Map<ProductVariantSearchIndex>(x 
                            => x.AutoMap().Properties(ps 
                                => ps.Completion(c 
                                    => c.Name(n => n.Suggest))))
            );
        }
        // public async Task ElasticSaveManyAsync(T[] types)
        // {
        //     _elasticCacheProduct.AddRange(types);
        //     var result = await _elasticClient.IndexManyAsync(types);
        //     if (result.Errors)
        //     {
        //         // the response can be inspected for errors
        //         foreach (var itemWithError in result.ItemsWithErrors)
        //         {
        //             throw new Exception();
        //         }
        //     }
        // }

        public async Task ElasticSaveBulkAsync(T[] types, string index)
        {
            // await _elasticClient.DeleteByQueryAsync<T>(del => del
            //     .Query(q => q.QueryString(qs=>qs.Query("*")))
            // );

            // _elasticCache.AddRange(types);
            // _logger.LogInformation($"Elastic search cache count {_elasticCache.Count}");
            // _logger.LogInformation($"Elastic search cache type {_elasticCache.GetType()}");
            Console.WriteLine("Indexing " + types.Length + "objects of type " + types.GetType() + "| Index: " +index);
            var result = await _elasticClient.BulkAsync(b => b.Index(index).IndexMany(types));
            if (result.Errors)
            {
                // the response can be inspected for errors
                foreach (var itemWithError in result.ItemsWithErrors)
                {
                    throw new Exception();
                }
            }
        }
        
        public async Task ElasticDeleteSingleAsync(T type, string index)
        {
            var response = await _elasticClient.DeleteAsync<T>(type, u => u.Index(index));
            // Console.WriteLine("ElasticDeleteSingleAsync: Type: " + type.GetType() + " || Delete");
            _logger.LogInformation("ElasticDeleteSingleAsync: Type: " + type.GetType() + " || Delete");

            if (!response.IsValid)
            {
                _logger.LogWarning("ElasticDeleteSingleAsync: Problem: " + response.DebugInformation);
                // throw new Exception(response.Result.ToString());
            }
        }

        public Notification GetNotificationInfoFromUserId(string userId)
        {
            return _identityAndProductDbContext.Notification
                .FirstOrDefault(noti => noti.UserId == userId);
        }
    }
}