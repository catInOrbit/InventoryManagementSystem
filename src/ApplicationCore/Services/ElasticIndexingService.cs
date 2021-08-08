using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using InventoryManagementSystem.ApplicationCore.Entities;
using InventoryManagementSystem.ApplicationCore.Entities.Orders;
using InventoryManagementSystem.ApplicationCore.Entities.Orders.Status;
using InventoryManagementSystem.ApplicationCore.Entities.Products;
using InventoryManagementSystem.ApplicationCore.Entities.SearchIndex;
using InventoryManagementSystem.ApplicationCore.Interfaces;

namespace InventoryManagementSystem.ApplicationCore.Services
{
    public class ElasticIndexingService<T> where T: BaseSearchIndex
    {
        private readonly IAsyncRepository<PurchaseOrder> _poRepository;
        private readonly IAsyncRepository<GoodsReceiptOrder> _grRepository;
        public readonly IAsyncRepository<GoodsIssueOrder> _giRepository;
        private readonly IAsyncRepository<StockTakeOrder> _stRepository;
        private readonly IAsyncRepository<Product> _productRepository;
        private readonly IAsyncRepository<ProductVariant> _productVariantRepository;

        public ElasticIndexingService(IAsyncRepository<PurchaseOrder> poRepository, IAsyncRepository<GoodsReceiptOrder> grRepository, IAsyncRepository<StockTakeOrder> stRepository, IAsyncRepository<GoodsIssueOrder> giRepository, IAsyncRepository<Product> productRepository, IAsyncRepository<ProductVariant> productVariantRepository)
        {
            _poRepository = poRepository;
            _grRepository = grRepository;
            _stRepository = stRepository;
            _giRepository = giRepository;
            _productRepository = productRepository;
            _productVariantRepository = productVariantRepository;
        }

        public ElasticIndexingService( IAsyncRepository<GoodsReceiptOrder> grRepository)
        {
            _grRepository = grRepository;
        }
        
        public ElasticIndexingService(IAsyncRepository<PurchaseOrder> poRepository)
        {
            _poRepository = poRepository;
        }
        
        public ElasticIndexingService(IAsyncRepository<GoodsIssueOrder> giRepository)
        {
            _giRepository = giRepository;
        }
        
        public ElasticIndexingService(IAsyncRepository<StockTakeOrder> stRepository)
        {
            _stRepository =stRepository;
        }
        
        public ElasticIndexingService(IAsyncRepository<Product> productRepository)
        {
            _productRepository = productRepository;
        }
        
        public ElasticIndexingService(IAsyncRepository<ProductVariant> productVariantRepository)
        {
            _productVariantRepository = productVariantRepository;
        }
        
           public async Task<PagingOption<PurchaseOrderSearchIndex>> IndexPurchasingOrder( PagingOption<PurchaseOrderSearchIndex> pagingOption,  CancellationToken cancellationToken = default)
            {
                List<PurchaseOrder> pos;
                var posDBAll = (await _poRepository.ListAllAsync(new PagingOption<PurchaseOrder>(0, 0))).ResultList;
                
                pos = posDBAll.
                    Where(variant => variant.Transaction.TransactionRecord.Count > 0 && variant.Transaction.TransactionStatus!=false 
                        && variant.Transaction.CurrentType!=TransactionType.Deleted
                    ).ToList();

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
                
                pagingOption.ExecuteResourcePaging();
                return pagingOption;
            }
           
           public async Task<PagingOption<GoodsReceiptOrderSearchIndex>> IndexReceivingOrder(PagingOption<GoodsReceiptOrderSearchIndex> pagingOption, CancellationToken cancellationToken = default)
           {
               var rosDbAll = (await _grRepository.ListAllAsync(new PagingOption<GoodsReceiptOrder>(0, 0))).ResultList;
               List<GoodsReceiptOrder> ros = rosDbAll.
                   Where(variant => variant.Transaction.TransactionRecord.Count > 0 &&variant.Transaction.TransactionStatus!=false && variant.Transaction.CurrentType!=TransactionType.Deleted).ToList();
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

               pagingOption.ExecuteResourcePaging();
               return pagingOption;
           }
           
             public async Task<PagingOption<GoodsIssueSearchIndex>> IndexGoodsIssue(PagingOption<GoodsIssueSearchIndex> pagingOption, CancellationToken cancellationToken = default)
             {
                 var gisDbAll = (await _giRepository.ListAllAsync(new PagingOption<GoodsIssueOrder>(0, 0))).ResultList;
            List<GoodsIssueOrder> gis =gisDbAll.
                Where(variant => variant.Transaction.TransactionRecord.Count > 0 &&variant.Transaction.TransactionStatus!=false && variant.Transaction.CurrentType!=TransactionType.Deleted).
                ToList();
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
            
            pagingOption.ExecuteResourcePaging();
            return pagingOption;
        }

        public async Task<PagingOption<StockTakeSearchIndex>> IndexStockTake(PagingOption<StockTakeSearchIndex> pagingOption, CancellationToken cancellationToken = default)
        {
            var stsDbAll = (await _stRepository.ListAllAsync(new PagingOption<StockTakeOrder>(0, 0))).ResultList;
            
            List<StockTakeOrder> sts = stsDbAll.
                Where(variant =>variant.Transaction.TransactionRecord.Count > 0 && variant.Transaction.TransactionStatus!=false && variant.Transaction.CurrentType!=TransactionType.Deleted).ToList();
            
            sts = sts.OrderByDescending(e =>
                e.Transaction.TransactionRecord[e.Transaction.TransactionRecord.Count - 1].Date).ToList();
            
            
            foreach (var st in sts)
            {
                try
                {
                    pagingOption.ResultList.Add(IndexingHelper.StockTakeSearchIndex(st));
                }
                catch (Exception e)
                {
                    Console.WriteLine(st.Id);
                    Console.WriteLine(e);
                    throw;
                }
            }
            pagingOption.ExecuteResourcePaging();
            return pagingOption;
        }
        
             public async Task<PagingOption<ProductSearchIndex>> IndexProduct(PagingOption<ProductSearchIndex> pagingOption, CancellationToken cancellationToken = default)
             {
                 var productsDBAll = (await _productRepository.ListAllAsync(new PagingOption<Product>(0, 0)))
                     .ResultList;
                    List<Product> products = productsDBAll.Where(product =>
                        product.Transaction.TransactionRecord.Count > 0 && product.Transaction.TransactionStatus != false &&
                        product.Transaction.CurrentType != TransactionType.Deleted).ToList();
        
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
        
                public async Task<PagingOption<ProductVariantSearchIndex>> IndexProductVariant(PagingOption<ProductVariantSearchIndex> pagingOption, CancellationToken cancellationToken = default)
                {
                    var productVariantDBAll =
                        (await _productVariantRepository.ListAllAsync(new PagingOption<ProductVariant>(0, 0)))
                        .ResultList;
                    List<ProductVariant> variants =  productVariantDBAll.
                        Where(variant => variant.Transaction.TransactionRecord.Count > 0 && variant.Transaction.TransactionStatus!=false && variant.Transaction.CurrentType!=TransactionType.Deleted).ToList();
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
                
    }
}