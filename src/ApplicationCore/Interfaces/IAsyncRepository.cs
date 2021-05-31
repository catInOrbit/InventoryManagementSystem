using Ardalis.Specification;
using InventoryManagementSystem.ApplicationCore.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using InventoryManagementSystem.ApplicationCore.Entities;
using InventoryManagementSystem.ApplicationCore.Entities.Orders;
using InventoryManagementSystem.ApplicationCore.Entities.Products;

namespace InventoryManagementSystem.ApplicationCore.Interfaces
{
    public interface IAsyncRepository<T> where T : BaseEntity
    {
        Task<T> GetByIdAsync(string id, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<ProductIndex>> GetProductForELIndexAsync(CancellationToken cancellationToken = default);
        PriceQuoteOrder GetPriceQuoteByNumber(string priceQuoteNumber,  CancellationToken cancellationToken = default);
        PurchaseOrder GetPurchaseOrderByNumber(string purchaseOrderNumber,  CancellationToken cancellationToken = default);

        Task<IReadOnlyList<T>> ListAllAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<Product>> ListAllProductAsync(CancellationToken cancellationToken = default);
        Task<IReadOnlyList<T>> ListAsync(ISpecification<T> spec, CancellationToken cancellationToken = default);
        Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
        Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
        Task DeleteAsync(T entity, CancellationToken cancellationToken = default);
        
        Task DeletePurchaseOrderAsync(PurchaseOrder entity, CancellationToken cancellationToken = default);

        Task<int> CountAsync(ISpecification<T> spec, CancellationToken cancellationToken = default);
        Task<T> FirstAsync(ISpecification<T> spec, CancellationToken cancellationToken = default);
        Task<T> FirstOrDefaultAsync(ISpecification<T> spec, CancellationToken cancellationToken = default);
        
        Task ElasticSaveSingleAsync(T product);
        Task ElasticSaveManyAsync(T[] products);
        Task ElasticSaveBulkAsync(T[] products);
    }
}
