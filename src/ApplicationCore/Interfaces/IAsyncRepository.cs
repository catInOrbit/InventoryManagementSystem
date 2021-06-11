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

        Task<PagingOption<ProductSearchIndex>> GetProductForELIndexAsync(CancellationToken cancellationToken = default);
        Task<PagingOption<PurchaseOrderSearchIndex>> GetPOForELIndexAsync(CancellationToken cancellationToken = default);
        Task<PagingOption<GoodsReceiptOrderSearchIndex>> GetROForELIndexAsync(CancellationToken cancellationToken = default);
        
        Task<PagingOption<GoodsIssueSearchIndex>> GetGIForELIndexAsync(CancellationToken cancellationToken = default);

        Task<PagingOption<StockTakeSearchIndex>> GetSTForELIndexAsync(CancellationToken cancellationToken = default);

        PriceQuoteOrder GetPriceQuoteByNumber(string priceQuoteNumber,  CancellationToken cancellationToken = default);
        PurchaseOrder GetPurchaseOrderByNumber(string purchaseOrderNumber,  CancellationToken cancellationToken = default);
        GoodsReceiptOrder GetReceivingOrderByNumber(string receiveOrderNumber,  CancellationToken cancellationToken = default);
        GoodsIssueOrder GetGoodsIssueOrderByNumber(string goodsIssueOrderNumber,  CancellationToken cancellationToken = default);

        // Task<IReadOnlyList<T>> ListAllAsync(CancellationToken cancellationToken = default);
        Task<PagingOption<T>> ListAllAsync(PagingOption<T> pagingOption, CancellationToken cancellationToken = default);

        Task<IEnumerable<Product>> ListAllProductAsync(CancellationToken cancellationToken = default);
        Task<IReadOnlyList<T>> ListAsync(ISpecification<T> spec, CancellationToken cancellationToken = default);
        Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
        Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
        Task DeleteAsync(T entity, CancellationToken cancellationToken = default);
        
        Task DeletePurchaseOrderAsync(PurchaseOrder entity, CancellationToken cancellationToken = default);

        Task<int> CountAsync(ISpecification<T> spec, CancellationToken cancellationToken = default);
        Task<T> FirstAsync(ISpecification<T> spec, CancellationToken cancellationToken = default);
        Task<T> FirstOrDefaultAsync(ISpecification<T> spec, CancellationToken cancellationToken = default);
        
        Task ElasticSaveSingleAsync(T types);
        Task ElasticSaveManyAsync(T[] types);
        Task ElasticSaveBulkAsync(T[] types, string index);
    }
}
