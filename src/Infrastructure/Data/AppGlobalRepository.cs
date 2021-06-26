using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.Specification;
using Elasticsearch.Net;
using Infrastructure.Identity.DbContexts;
using InventoryManagementSystem.ApplicationCore.Entities;
using InventoryManagementSystem.ApplicationCore.Entities.Orders;
using InventoryManagementSystem.ApplicationCore.Entities.Orders.Status;
using InventoryManagementSystem.ApplicationCore.Entities.Products;
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

        private List<T> _elasticCache = new List<T>();
        private List<ProductSearchIndex> _elasticCacheProductSearchIndex = new List<ProductSearchIndex>();

        public AppGlobalRepository(IdentityAndProductDbContext identityAndProductDbContext, IElasticClient elasticClient)
        {
            _identityAndProductDbContext = identityAndProductDbContext;
            _elasticClient = elasticClient;
        }

        public async Task<T> GetByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            var keyValues = new object[] { id };
            return await _identityAndProductDbContext.Set<T>().FindAsync(keyValues, cancellationToken);
        }

        public async Task<PagingOption<ProductSearchIndex>> GetProductForELIndexAsync(PagingOption<ProductSearchIndex> pagingOption, CancellationToken cancellationToken = default)
        {
            var variants =  await _identityAndProductDbContext.ProductVariant.ToListAsync(cancellationToken);
            foreach (var productVariant in variants)
            {
                try
                {
                 
                    var index = IndexingHelper.ProductSearchIndex(productVariant);
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

        public async Task<PagingOption<PurchaseOrderSearchIndex>> GetPOForELIndexAsync(PagingOption<PurchaseOrderSearchIndex> pagingOption, POSearchFilter poSearchFilter,  CancellationToken cancellationToken = default)
        {

            var pos = await _identityAndProductDbContext.PurchaseOrder.ToListAsync();
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


        // public async Task<List<PurchaseOrder>> PurchaseOrderFiltering(POSearchFilter poSearchFilter, CancellationToken cancellationToken)
        // {
        //     var pos = await _identityAndProductDbContext.Set<PurchaseOrder>().Where(po =>
        //             (poSearchFilter.FromStatus == null ||
        //              
        //              po.PurchaseOrderStatus == (PurchaseOrderStatusType) poSearchFilter.Status) &&
        //             
        //             
        //             (poSearchFilter.FromDeliveryDate == null ||
        //              (po.DeliveryDate >= DateTime.Parse(poSearchFilter.FromDeliveryDate) &&
        //               po.DeliveryDate <= DateTime.Parse(poSearchFilter.ToDeliveryDate))) &&
        //             (poSearchFilter.FromCreatedDate == null ||
        //              (po.Transaction.CreatedDate >= DateTime.Parse(poSearchFilter.FromCreatedDate) &&
        //               po.Transaction.CreatedDate <= DateTime.Parse(poSearchFilter.ToCreatedDate))) &&
        //             (poSearchFilter.FromTotalOrderPrice == null ||
        //              (po.TotalOrderAmount >= Decimal.Parse(poSearchFilter.FromTotalOrderPrice) &&
        //               po.TotalOrderAmount <= Decimal.Parse(poSearchFilter.ToTotalOrderPrice))) &&
        //             (poSearchFilter.SupplierId == null || po.SupplierId == poSearchFilter.SupplierId) &&
        //             (poSearchFilter.CreatedByName == null || po.Transaction.CreatedBy.Fullname == poSearchFilter.CreatedByName)
        //             &&
        //             (poSearchFilter.FromModifiedDate == null ||
        //              (po.Transaction.ModifiedDate >= DateTime.Parse(poSearchFilter.FromModifiedDate) &&
        //               po.Transaction.ModifiedDate <= DateTime.Parse(poSearchFilter.ToModifiedDate)))
        //             )
        //         .ToListAsync(cancellationToken);
        //     return pos;
        // }
        
        
        public List<PurchaseOrderSearchIndex> PurchaseOrderIndexFiltering(List<PurchaseOrderSearchIndex> resource, POSearchFilter poSearchFilter, CancellationToken cancellationToken  = default)
        {
            var pos = resource.Where(po =>
                    ( (poSearchFilter.FromStatus  == null ||
                      (int)(ParseEnum<PurchaseOrderStatusType>(po.Status))  >= Int32.Parse(poSearchFilter.FromStatus)
                        &&
                      (int)(ParseEnum<PurchaseOrderStatusType>(po.Status))  <= Int32.Parse(poSearchFilter.ToStatus))
                      
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
                ))
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
            
            var ros = await _identityAndProductDbContext.Set<GoodsReceiptOrder>().ToListAsync(cancellationToken);

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
            var gis = await _identityAndProductDbContext.Set<GoodsIssueOrder>().ToListAsync(cancellationToken);
           
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
            
            var sts = await _identityAndProductDbContext.Set<StockTakeOrder>().ToListAsync(cancellationToken);
            
            foreach (var st in sts)
            {
                StockTakeSearchIndex index; 
                try
                {
                    index = new StockTakeSearchIndex
                    {
                        Id = st.Id,
                        CreatedByName = st.Transaction.CreatedBy.Fullname,
                        Status = st.StockTakeOrderType.ToString(),
                        CreatedDate = st.Transaction.CreatedDate,
                        ModifiedDate = st.Transaction.ModifiedDate
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

        public async Task<PagingOption<T>> ListAllAsync(PagingOption<T> pagingOption, CancellationToken cancellationToken = default)
        {
            pagingOption.ResultList = await _identityAndProductDbContext.Set<T>().ToListAsync(cancellationToken);
            pagingOption.ExecuteResourcePaging();
            return pagingOption;
        }

        public async Task<PagingOption<StockOnhandReport>> GenerateOnHandReport(PagingOption<StockOnhandReport> pagingOption, CancellationToken cancellationToken = default)
        {
            var list = await _identityAndProductDbContext.Set<Package>().Skip(pagingOption.SkipValue)
                .Take(pagingOption.SizePerPage).OrderByDescending(pac => pac.ImportedDate).ToListAsync();

            foreach (var package in list)
            {
                try
                {
                    var stohReport = new StockOnhandReport
                    {
                        Value = (package.TotalImportPrice!= null && package.TotalImportQuantity != null) ?  package.TotalImportPrice * package.TotalImportQuantity : -1,
                        Date = package.ImportedDate,
                        ProductName = (package.ProductVariant != null) ? package.ProductVariant.Name : "",
                        StorageQuantity = (package.ProductVariant != null) ? package.ProductVariant.StorageQuantity : -1
                    };
                
                    pagingOption.ResultList.Add(stohReport);
                }
                catch (Exception e)
                {
                    Console.WriteLine(package.Id);
                    throw;
                }
                
            }

            pagingOption.ExecuteResourcePaging(list.Count);
            return pagingOption;
        }

        public async Task<PagingOption<StockTakeReport>> GenerateStockTakeReport(PagingOption<StockTakeReport> pagingOption, CancellationToken cancellationToken = default)
        {
            var list = await _identityAndProductDbContext.Set<StockTakeItem>().Skip(pagingOption.SkipValue)
                .Take(pagingOption.SizePerPage).OrderByDescending(item =>  item.StockTakeOrder.Transaction.CreatedDate).ToListAsync();

            foreach (var stockTakeItem in list)
            {
                var stockTakeReport = new StockTakeReport()
                {
                    StockTakeDate = stockTakeItem.StockTakeOrder.Transaction.CreatedDate,
                    ProductName = stockTakeItem.ProductVariant.Name,
                    StorageQuantity = stockTakeItem.ProductVariant.StorageQuantity,
                    ActualQuantity = stockTakeItem.ActualQuantity,
                    Value = stockTakeItem.ProductVariant.StorageQuantity * stockTakeItem.ProductVariant.Price
                };
                
                pagingOption.ResultList.Add(stockTakeReport);
            }

            pagingOption.ExecuteResourcePaging(list.Count);
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
            throw new System.NotImplementedException();
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
            throw new System.NotImplementedException();
        }

        public Task<T> FirstAsync(ISpecification<T> spec, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

        public Task<T> FirstOrDefaultAsync(ISpecification<T> spec, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }
        
        public async Task ElasticSaveSingleAsync(bool isSavingNew, T type, string index)
        {
            // if (_elasticCache.Any(p => p.Id == type.Id))
            // {
            //     await _elasticClient.UpdateAsync<T>(type, u => u.Doc(type));
            // }

            if (!isSavingNew)
            {
                Console.WriteLine("ElasticSaveSingleAsync: Type: " + type.GetType() + " || Update");
                var response = await _elasticClient.UpdateAsync<T>(type, u => u.Index(index).Doc(type));
                if (!response.IsValid)
                    throw new Exception();
            }

            else
            {
                Console.WriteLine("ElasticSaveSingleAsync: Type: " + type.GetType() + " || AddNew");
                // _elasticCache.Add(type);
                // await _elasticClient.IndexDocumentAsync<T>(type);
                var respone = await _elasticClient.IndexAsync(type, i => i.Index(index));
                if (!respone.IsValid)
                    throw new Exception();
            }
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
            Console.WriteLine("ElasticDeleteSingleAsync: Type: " + type.GetType() + " || Delete");

            if (!response.IsValid)
                throw new Exception();
        }

        public Notification GetNotificationInfoFromUserId(string userId)
        {
            return _identityAndProductDbContext.Notification
                .FirstOrDefault(noti => noti.UserId == userId);
        }
    }
}