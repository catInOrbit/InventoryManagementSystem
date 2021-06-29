using Ardalis.Specification;
using InventoryManagementSystem.ApplicationCore.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using InventoryManagementSystem.ApplicationCore.Entities;
using InventoryManagementSystem.ApplicationCore.Entities.Orders;
using InventoryManagementSystem.ApplicationCore.Entities.Products;
using InventoryManagementSystem.ApplicationCore.Entities.SearchIndex;

namespace InventoryManagementSystem.ApplicationCore.Interfaces
{
    public interface IAsyncRepository<T> where T : BaseEntity
    {
        Task<T> GetByIdAsync(string id, CancellationToken cancellationToken = default);

        Task<PagingOption<ProductSearchIndex>> GetProductForELIndexAsync(PagingOption<ProductSearchIndex> pagingOption, CancellationToken cancellationToken = default);
        Task<PagingOption<PurchaseOrderSearchIndex>> GetPOForELIndexAsync(PagingOption<PurchaseOrderSearchIndex> pagingOption,POSearchFilter poSearchFilter,CancellationToken cancellationToken = default);
        Task<PagingOption<GoodsReceiptOrderSearchIndex>> GetROForELIndexAsync(PagingOption<GoodsReceiptOrderSearchIndex> pagingOption, ROSearchFilter roSearchFilter, CancellationToken cancellationToken = default);
        
        Task<PagingOption<GoodsIssueSearchIndex>> GetGIForELIndexAsync(PagingOption<GoodsIssueSearchIndex> pagingOption, GISearchFilter searchFilter, CancellationToken cancellationToken = default);

        Task<PagingOption<StockTakeSearchIndex>> GetSTForELIndexAsync(PagingOption<StockTakeSearchIndex> pagingOption,STSearchFilter stSearchFilter, CancellationToken cancellationToken = default);
        
        // PriceQuoteOrder GetPriceQuoteByNumber(string priceQuoteNumber,  CancellationToken cancellationToken = default);
        PurchaseOrder GetPurchaseOrderByNumber(string purchaseOrderId,  CancellationToken cancellationToken = default);
        GoodsReceiptOrder GetReceivingOrderByNumber(string receiveOrderId,  CancellationToken cancellationToken = default);
        GoodsIssueOrder GetGoodsIssueOrderByNumber(string goodsIssueOrderId,  CancellationToken cancellationToken = default);
        Task<List<Package>> GetPackagesFromProductVariantId(string productVariantId,  CancellationToken cancellationToken = default);

        List<PurchaseOrderSearchIndex> PurchaseOrderIndexFiltering(List<PurchaseOrderSearchIndex> resource,
            POSearchFilter poSearchFilter, CancellationToken cancellationToken);
        
        List<GoodsReceiptOrderSearchIndex> ReceivingOrderIndexFiltering(List<GoodsReceiptOrderSearchIndex> resource,
            ROSearchFilter roSearchFilter, CancellationToken cancellationToken);
        
        List<GoodsIssueSearchIndex> GoodsIssueIndexFiltering(List<GoodsIssueSearchIndex> resource,
            GISearchFilter giSearchFilter, CancellationToken cancellationToken);
        
        List<ProductSearchIndex> ProductIndexFiltering(List<ProductSearchIndex> resource,
            ProductSearchFilter productSearchFilter, CancellationToken cancellationToken);
        
        List<StockTakeSearchIndex> StockTakeIndexFiltering(List<StockTakeSearchIndex> resource,
            STSearchFilter stSearchFilter, CancellationToken cancellationToken);

        // Task<IReadOnlyList<T>> ListAllAsync(CancellationToken cancellationToken = default);
        Task<PagingOption<T>> ListAllAsync(PagingOption<T> pagingOption, CancellationToken cancellationToken = default);
        Task<PagingOption<StockOnhandReport>> GenerateOnHandReport(PagingOption<StockOnhandReport> pagingOption, CancellationToken cancellationToken = default);
        Task<PagingOption<StockTakeReport>> GenerateStockTakeReport(PagingOption<StockTakeReport> pagingOption, CancellationToken cancellationToken = default);
        Task<PagingOption<ProductVariant>> GenerateTopSellingYearReport(PagingOption<ProductVariant> pagingOption, CancellationToken cancellationToken = default);
        Task<PagingOption<ProductVariant>> GenerateTopSellingCurrentMonthReport(PagingOption<ProductVariant> pagingOption, CancellationToken cancellationToken = default);


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

        Notification GetNotificationInfoFromUserId(string userId);

    }
}
