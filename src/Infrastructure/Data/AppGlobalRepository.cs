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
using InventoryManagementSystem.ApplicationCore.Entities.Products;
using InventoryManagementSystem.ApplicationCore.Entities.SearchIndex;
using InventoryManagementSystem.ApplicationCore.Interfaces;
using Microsoft.EntityFrameworkCore;
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

        public async Task<List<ProductSearchIndex>> GetProductForELIndexAsync(PagingOption<T> pagingOption, CancellationToken cancellationToken = default)
        {
            // var products= await _identityAndProductDbContext.Set<ProductVariant>().Select(p=> new {p.Id, p.Name}).ToListAsync(cancellationToken);
            // var products= await _identityAndProductDbContext.Set<ProductVariant>().ToListAsync(cancellationToken);
            List<ProductVariant> variants;
            if(!pagingOption.NoPaging)
                variants =  await _identityAndProductDbContext.ProductVariant
                    .Take(pagingOption.SizePerPage).ToListAsync(cancellationToken);
            variants =  await _identityAndProductDbContext.ProductVariant.ToListAsync(cancellationToken);
            
            List<ProductSearchIndex> psis = new List<ProductSearchIndex>();
            foreach (var productVariant in variants)
            {
                try
                {
                    string nameConcat = productVariant.Name;
                    foreach (var productVariantVariantValue in productVariant.VariantValues)
                    {
                        nameConcat += "-" + productVariantVariantValue.Value.Trim();
                    }
                
                    var index = new ProductSearchIndex
                    {
                        Id = productVariant.Id,
                        Name = nameConcat,
                        ProductId = productVariant.ProductId,
                        VariantId = productVariant.Id,
                        Catagory = _identityAndProductDbContext.Product.Where(pro => pro.Id == productVariant.ProductId).FirstOrDefault()?.Category.CategoryName,
                        Quantity = productVariant.StorageQuantity,
                        ModifiedDate = productVariant.ModifiedDate
                    };
                
                    psis.Add(index);
                }
                catch (Exception e)
                {
                    Console.WriteLine(productVariant.Id);
                    Console.WriteLine(e);
                    throw;
                }
               
            }            
            // List<ProductSearchIndex> indices = new List<ProductSearchIndex>();
            // foreach (var productVariant in products)
            // {
            //     var index = new ProductSearchIndex
            //     {
            //         Id = Guid.NewGuid().ToString(),
            //         ProductId = productVariant.ProductId,
            //         Name = productVariant.Name,
            //         Price = productVariant.Price,
            //         ProductVariants = products
            //     };
            //     indices.Add(index);
            // }
            
            // return indices;
            // var prodcuctIndices = new List<ProductVariant>();
            // foreach (var product in products)
            // {
            //     ProductIndex productIndex = new ProductIndex
            //     {
            //         Id = product.Id,
            //         Name = product.Name
            //     };
            //     prodcuctIndices.Add(productIndex);
            // }
            // return productVariants;
            return psis;
        }

        public async Task<List<PurchaseOrderSearchIndex>> GetPOForELIndexAsync(PagingOption<T> pagingOption, CancellationToken cancellationToken = default)
        {
            List<PurchaseOrder> pos;
            if(!pagingOption.NoPaging)
                pos = await _identityAndProductDbContext.Set<PurchaseOrder>().Skip(pagingOption.SkipValue)
                    .Take(pagingOption.SizePerPage).Where(po => po.Transaction.TransactionStatus==true).ToListAsync(cancellationToken);
            pos = await _identityAndProductDbContext.Set<PurchaseOrder>().Where(po => po.Transaction.TransactionStatus==true).ToListAsync(cancellationToken);
            
            List<PurchaseOrderSearchIndex> posi = new List<PurchaseOrderSearchIndex>();
            foreach (var po in pos)
            {
                PurchaseOrderSearchIndex index; 
                try
                {
                    index = new PurchaseOrderSearchIndex
                    {
                        Id = po.Id,
                        SupplierName = (po.Supplier!=null) ? po.Supplier.SupplierName : "",
                        PurchaseOrderNumber = (po.PurchaseOrderNumber !=null) ? po.PurchaseOrderNumber : "",
                        Status = (po.PurchaseOrderStatus.GetStringValue()!=null) ? po.PurchaseOrderStatus.GetStringValue() : "",
                        CreatedDate = po.Transaction.CreatedDate,
                        DeliveryDate = po.DeliveryDate ,
                        TotalPrice = (po.TotalOrderAmount!=null) ? po.TotalOrderAmount : 0,
                        ConfirmedByName = (po.Transaction.CreatedBy!=null) ? po.Transaction.CreatedBy.Fullname : "" 
                    };
                    posi.Add(index);
                }
                catch (Exception e)
                {
                    Console.WriteLine(po.Id);
                    throw;
                }
               
            }

            return posi;
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

        public async Task<List<GoodsReceiptOrderSearchIndex>> GetROForELIndexAsync(PagingOption<T> pagingOption, CancellationToken cancellationToken = default)
        {
            List<GoodsReceiptOrder> ros;
            if (!pagingOption.NoPaging)
            {
                ros = await _identityAndProductDbContext.Set<GoodsReceiptOrder>().Skip(pagingOption.SkipValue)
                    .Take(pagingOption.SizePerPage).ToListAsync(cancellationToken);
            }
            ros = await _identityAndProductDbContext.Set<GoodsReceiptOrder>().ToListAsync(cancellationToken);
            
            List<GoodsReceiptOrderSearchIndex> rosi = new List<GoodsReceiptOrderSearchIndex>();
            foreach (var ro in ros)
            {
                GoodsReceiptOrderSearchIndex index; 
                try
                {
                    index = new GoodsReceiptOrderSearchIndex
                    {
                        Id = ro.Id,
                        purchaseOrderId = (ro.PurchaseOrderId!=null) ? ro.PurchaseOrderId : "",
                        supplierName = (ro.Supplier!=null) ? ro.Supplier.SupplierName : "",
                        createdBy = (ro.Transaction.CreatedBy!=null) ? ro.Transaction.CreatedBy.Fullname : "" ,
                        receiptId = (ro.GoodsReceiptOrderNumber !=null) ? ro.GoodsReceiptOrderNumber : ""  ,
                        createdDate = ro.Transaction.CreatedDate.ToShortDateString()
                    };
                    rosi.Add(index);
                }
                catch (Exception e)
                {
                    Console.WriteLine(ro.Id);
                    throw;
                }
               
            }

            return rosi;
        }

        public async Task<List<GoodsIssueSearchIndex>> GetGIForELIndexAsync(PagingOption<T> pagingOption, CancellationToken cancellationToken = default)
        {
            List<GoodsIssueOrder> gis;
            if(!pagingOption.NoPaging)
                gis = await _identityAndProductDbContext.Set<GoodsIssueOrder>().Skip(pagingOption.SkipValue)
                    .Take(pagingOption.SizePerPage).ToListAsync(cancellationToken);
            gis = await _identityAndProductDbContext.Set<GoodsIssueOrder>().ToListAsync(cancellationToken);
            List<GoodsIssueSearchIndex> gisi = new List<GoodsIssueSearchIndex>();
            foreach (var gi in gis)
            {
                GoodsIssueSearchIndex index; 
                try
                {
                    index = new GoodsIssueSearchIndex
                    {
                        Id = gi.Id,
                        GoodsIssueNumber = gi.GoodsIssueNumber, 
                        GoodsIssueRequestNumber = gi.RequestId,
                        Status = gi.GoodsIssueType.ToString(),
                        DeliveryDate = gi.DeliveryDate,
                        CreatedByName = (gi.Transaction!=null) ? gi.Transaction.CreatedBy.Fullname : "",
                    };
                    gisi.Add(index);
                }
                catch (Exception e)
                {
                    Console.WriteLine(gi.Id);
                    throw;
                }
            }
            return gisi;
        }

        public async Task<List<StockTakeSearchIndex>> GetSTForELIndexAsync(PagingOption<T> pagingOption, CancellationToken cancellationToken = default)
        {
            
            List<StockTakeOrder> sts;
            if(!pagingOption.NoPaging)
                sts = await _identityAndProductDbContext.Set<StockTakeOrder>().Skip(pagingOption.SkipValue)
                    .Take(pagingOption.SizePerPage).ToListAsync(cancellationToken);
            sts = await _identityAndProductDbContext.Set<StockTakeOrder>().ToListAsync(cancellationToken);
            
            List<StockTakeSearchIndex> stsi = new List<StockTakeSearchIndex>();
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
                    stsi.Add(index);
                }
                catch (Exception e)
                {
                    Console.WriteLine(st.Id);
                    Console.WriteLine(e);
                    throw;
                }
            }
            return stsi;
        }

        public PriceQuoteOrder GetPriceQuoteByNumber(string priceQuoteNumber, CancellationToken cancellationToken = default)
        {
            return _identityAndProductDbContext.PriceQuote.Where(pq => pq.PriceQuoteNumber == priceQuoteNumber).
                SingleOrDefault(pq => pq.PriceQuoteNumber == priceQuoteNumber);
        }
 
        public PurchaseOrder GetPurchaseOrderByNumber(string purchaseOrderNumber, CancellationToken cancellationToken = default)
        {
            return _identityAndProductDbContext.PurchaseOrder.Where(po => po.PurchaseOrderNumber == purchaseOrderNumber && po.Transaction.TransactionStatus == true).
                SingleOrDefault(po => po.PurchaseOrderNumber == purchaseOrderNumber);   
        }

        public GoodsReceiptOrder GetReceivingOrderByNumber(string receiveOrderNumber, CancellationToken cancellationToken = default)
        {
            return _identityAndProductDbContext.GoodsReceiptOrder.Where(go => go.GoodsReceiptOrderNumber == receiveOrderNumber).
                SingleOrDefault(po => po.GoodsReceiptOrderNumber == receiveOrderNumber);   
        }

        public GoodsIssueOrder GetGoodsIssueOrderByNumber(string goodsIssueOrderNumber, CancellationToken cancellationToken = default)
        {
            return _identityAndProductDbContext.GoodsIssueOrder.Where(go => go.GoodsIssueNumber == goodsIssueOrderNumber).
                SingleOrDefault(po => po.GoodsIssueNumber == goodsIssueOrderNumber);
        }

        public async Task<PagingOption<T>> ListAllAsync(PagingOption<T> pagingOption, CancellationToken cancellationToken = default)
        {
            if(pagingOption.NoPaging)
                pagingOption.Results = await _identityAndProductDbContext.Set<T>().ToListAsync(cancellationToken);
            pagingOption.Results = await _identityAndProductDbContext.Set<T>().Skip(pagingOption.SkipValue).Take(pagingOption.SizePerPage).ToListAsync(cancellationToken);
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
            var poItems = _identityAndProductDbContext.OrderItem.Where(poItem => poItem.OrderNumber == entity.Id);

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
        
        public async Task ElasticSaveSingleAsync(T types)
        {
            if (_elasticCache.Any(p => p.Id == types.Id))
            {
                await _elasticClient.UpdateAsync<T>(types, u => u.Doc(types));
            }
            else
            {
                _elasticCache.Add(types);
                await _elasticClient.IndexDocumentAsync<T>(types);
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

            _elasticCache.AddRange(types);
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
    }
}