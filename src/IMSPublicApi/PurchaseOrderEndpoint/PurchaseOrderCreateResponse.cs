using InventoryManagementSystem.ApplicationCore.Entities;
using InventoryManagementSystem.ApplicationCore.Entities.Orders;

namespace InventoryManagementSystem.PublicApi.PurchaseOrderEndpoint
{
    public class PurchaseOrderCreateResponse : BaseResponse
    {
        public PurchaseOrder PurchaseOrder { get; set; }
        
        public UserInfo CreatedByUserInfo { get; set; }

        public Supplier SupplierInfo { get; set; }

    }
}