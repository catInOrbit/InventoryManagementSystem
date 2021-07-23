﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.Specification;
using InventoryManagementSystem.ApplicationCore.Entities;
using InventoryManagementSystem.ApplicationCore.Entities.Orders;
using InventoryManagementSystem.ApplicationCore.Entities.Products;
using InventoryManagementSystem.ApplicationCore.Entities.Reports;
using InventoryManagementSystem.ApplicationCore.Entities.SearchIndex;
using InventoryManagementSystem.ApplicationCore.Extensions;

namespace InventoryManagementSystem.ApplicationCore.Interfaces
{
    public interface IAsyncRepository<T> where T : BaseEntity
    {
        Task<T> GetByIdAsync(string id, CancellationToken cancellationToken = default);

        Task<PagingOption<ProductSearchIndex>> GetProductForELIndexAsync(
            PagingOption<ProductSearchIndex> pagingOption, CancellationToken cancellationToken = default);
        Task<PagingOption<ProductVariantSearchIndex>> GetProductVariantForELIndexAsync(PagingOption<ProductVariantSearchIndex> pagingOption, CancellationToken cancellationToken = default);
        
        Task<PagingOption<PurchaseOrderSearchIndex>> GetPOForELIndexAsync(bool hideMergeStatus, PagingOption<PurchaseOrderSearchIndex> pagingOption,POSearchFilter poSearchFilter,CancellationToken cancellationToken = default);
        Task<PagingOption<GoodsReceiptOrderSearchIndex>> GetROForELIndexAsync(PagingOption<GoodsReceiptOrderSearchIndex> pagingOption, ROSearchFilter roSearchFilter, CancellationToken cancellationToken = default);
        List<MergedOrderIdList> GetMergedPurchaseOrders(string parentOrderId,CancellationToken cancellationToken = default);
        Task<PagingOption<GoodsIssueSearchIndex>> GetGIForELIndexAsync(PagingOption<GoodsIssueSearchIndex> pagingOption, GISearchFilter searchFilter, CancellationToken cancellationToken = default);

        Task<PagingOption<StockTakeSearchIndex>> GetSTForELIndexAsync(PagingOption<StockTakeSearchIndex> pagingOption,STSearchFilter stSearchFilter, CancellationToken cancellationToken = default);
        
        // PriceQuoteOrder GetPriceQuoteByNumber(string priceQuoteNumber,  CancellationToken cancellationToken = default);
        PurchaseOrder GetPurchaseOrderByNumber(string purchaseOrderId,  CancellationToken cancellationToken = default);
        GoodsReceiptOrder GetReceivingOrderByNumber(string receiveOrderId,  CancellationToken cancellationToken = default);
        GoodsIssueOrder GetGoodsIssueOrderByNumber(string goodsIssueOrderId,  CancellationToken cancellationToken = default);
        Task<PagingOption<Category>> GetCategory(PagingOption<Category> pagingOption, CancellationToken cancellationToken = default);

        Task<List<Package>> GetPackagesFromProductVariantId(string productVariantId,  CancellationToken cancellationToken = default);

        List<PurchaseOrderSearchIndex> PurchaseOrderIndexFiltering(List<PurchaseOrderSearchIndex> resource,
            POSearchFilter poSearchFilter, CancellationToken cancellationToken);
        
        List<GoodsReceiptOrderSearchIndex> ReceivingOrderIndexFiltering(List<GoodsReceiptOrderSearchIndex> resource,
            ROSearchFilter roSearchFilter, CancellationToken cancellationToken);
        
        List<GoodsIssueSearchIndex> GoodsIssueIndexFiltering(List<GoodsIssueSearchIndex> resource,
            GISearchFilter giSearchFilter, CancellationToken cancellationToken);
        
        List<ProductVariantSearchIndex> ProductVariantIndexFiltering(List<ProductVariantSearchIndex> resource,
            ProductVariantSearchFilter productSearchFilter, CancellationToken cancellationToken);
        
        List<Package> PackageIndexFiltering(List<Package> resource,
            PackageSearchFilter packageSearchFilter, CancellationToken cancellationToken);
        
        List<ProductSearchIndex> ProductIndexFiltering(List<ProductSearchIndex> resource,
            ProductSearchFilter productSearchFilter, CancellationToken cancellationToken);

        Task<PagingOption<Package>> GetPackages(PagingOption<Package> pagingOption,CancellationToken cancellationToken = default);
        List<StockTakeSearchIndex> StockTakeIndexFiltering(List<StockTakeSearchIndex> resource,
            STSearchFilter stSearchFilter, CancellationToken cancellationToken);

        // Task<IReadOnlyList<T>> ListAllAsync(CancellationToken cancellationToken = default);
        Task<PagingOption<T>> ListAllAsync(PagingOption<T> pagingOption, CancellationToken cancellationToken = default);
        
        Task<PagingOption<Supplier>> GetSuppliers(PagingOption<Supplier> pagingOption, CancellationToken cancellationToken = default);

        Task<PagingOption<StockOnhandReport>> GenerateOnHandReport(PagingOption<StockOnhandReport> pagingOption, CancellationToken cancellationToken = default);
        Task<PagingOption<StockTakeReport>> GenerateStockTakeReport(PagingOption<StockTakeReport> pagingOption, CancellationToken cancellationToken = default);
        Task<PagingOption<TopSellingReport>> GenerateTopSellingReport(ReportType reportType, PagingOption<TopSellingReport> pagingOption, CancellationToken cancellationToken = default);


        Task<IEnumerable<Product>> ListAllProductAsync(CancellationToken cancellationToken = default);
        Task<IReadOnlyList<T>> ListAsync(ISpecification<T> spec, CancellationToken cancellationToken = default);
        Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
        Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
        Task DeleteAsync(T entity, CancellationToken cancellationToken = default);
        
        Task DeletePurchaseOrderAsync(PurchaseOrder entity, CancellationToken cancellationToken = default);

        Task<int> CountAsync(ISpecification<T> spec, CancellationToken cancellationToken = default);
        Task<T> FirstAsync(ISpecification<T> spec, CancellationToken cancellationToken = default);
        Task<T> FirstOrDefaultAsync(ISpecification<T> spec, CancellationToken cancellationToken = default);
        
        Task ElasticSaveSingleAsync(bool isSavingNew, T types, string index);
        Task ElasticSaveManyAsync(T[] types);
        Task ElasticSaveBulkAsync(T[] types, string index);
        Task ElasticDeleteSingleAsync(T type, string index);

        Task ReIndexProduct();
        Task ReIndexProductVariant();

        Notification GetNotificationInfoFromUserId(string userId);

    }
}
