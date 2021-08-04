using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.Specification;
using InventoryManagementSystem.ApplicationCore.Entities;
using InventoryManagementSystem.ApplicationCore.Entities.Orders;
using InventoryManagementSystem.ApplicationCore.Entities.Products;
using InventoryManagementSystem.ApplicationCore.Entities.Reports;
using InventoryManagementSystem.ApplicationCore.Extensions;

namespace InventoryManagementSystem.ApplicationCore.Interfaces
{
    public interface IAsyncRepository<T> where T : BaseEntity
    {
        Task<T> GetByIdAsync(string id, CancellationToken cancellationToken = default);

        List<MergedOrderIdList> GetMergedPurchaseOrders(string parentOrderId,CancellationToken cancellationToken = default);
        
        GoodsIssueOrder GetGoodsIssueOrderByNumber(string goodsIssueOrderId,  CancellationToken cancellationToken = default);
        Task<List<GoodsReceiptOrder>> GetAllGoodsReceiptsOfPurchaseOrder(string purchaseOrderId,  CancellationToken cancellationToken = default);

        Task<PagingOption<Category>> GetCategory(PagingOption<Category> pagingOption, CancellationToken cancellationToken = default);

        Task<List<Package>> GetPackagesFromProductVariantId(string productVariantId,  CancellationToken cancellationToken = default);

        Task<PagingOption<Package>> GetPackages(PagingOption<Package> pagingOption,CancellationToken cancellationToken = default);
        Task<PagingOption<T>> ListAllAsync(PagingOption<T> pagingOption, CancellationToken cancellationToken = default);
        Task<List<Notification>> ListAllNotificationByChannel(string channels, CancellationToken cancellationToken = default);

        Task<PagingOption<Supplier>> GetSuppliers(PagingOption<Supplier> pagingOption, CancellationToken cancellationToken = default);

        Task<PagingOption<StockOnhandReport>> GenerateOnHandReport(PagingOption<StockOnhandReport> pagingOption, CancellationToken cancellationToken = default);
        Task<PagingOption<StockTakeReport>> GenerateStockTakeReport(PagingOption<StockTakeReport> pagingOption, CancellationToken cancellationToken = default);
        Task<PagingOption<TopSellingReport>> GenerateTopSellingReport(ReportType reportType, PagingOption<TopSellingReport> pagingOption, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<T>> ListAsync(ISpecification<T> spec, CancellationToken cancellationToken = default);
        Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
        Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
        Task DeleteAsync(T entity, CancellationToken cancellationToken = default);
        
        Notification GetNotificationInfoFromUserId(string userId);

    }
}
