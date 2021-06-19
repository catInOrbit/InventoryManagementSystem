using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InventoryManagementSystem.ApplicationCore.Entities;
using InventoryManagementSystem.ApplicationCore.Entities.Orders;
using InventoryManagementSystem.ApplicationCore.Entities.Products;
using InventoryManagementSystem.ApplicationCore.Interfaces;

namespace Infrastructure.Services
{
    public class ProductStrategyService
    {
        private IAsyncRepository<GoodsReceiptOrder> _roAsyncRepository;
        private IAsyncRepository<GoodsReceiptOrderItem> _goOrderItemsAsyncRepository;

        public ProductStrategyService(IAsyncRepository<GoodsReceiptOrder> roAsyncRepository, IAsyncRepository<GoodsReceiptOrderItem> goOrderItemsAsyncRepository)
        {
            _roAsyncRepository = roAsyncRepository;
            _goOrderItemsAsyncRepository = goOrderItemsAsyncRepository;
        }


        public async Task<List<GoodsReceiptOrder>> GetFIFOROFromProducts(List<string> productVariantIds)
        {
            List<GoodsReceiptOrderItem> orderItems = new List<GoodsReceiptOrderItem>();
            var pagingOption = await _goOrderItemsAsyncRepository.ListAllAsync(new PagingOption<GoodsReceiptOrderItem>(0,0));
            orderItems = pagingOption.ResultList.ToList();
            
            List<GoodsReceiptOrder> receiptOrders = new List<GoodsReceiptOrder>();
            
            foreach (var orderItem in orderItems)
            {
                if (productVariantIds.Contains(orderItem.ProductVariantId))
                {
                    var receiptOrder = await _roAsyncRepository.GetByIdAsync(orderItem.GoodsReceiptOrderId); 
                    if(!receiptOrders.Contains(receiptOrder))
                        receiptOrders.Add(receiptOrder);
                }
            }
            receiptOrders.OrderBy(ro => ro.ReceivedDate);
            return receiptOrders;
        }
    }
}