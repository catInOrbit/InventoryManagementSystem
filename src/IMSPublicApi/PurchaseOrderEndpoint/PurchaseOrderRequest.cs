using System;
using System.Collections.Generic;
using InventoryManagementSystem.ApplicationCore.Entities;

namespace InventoryManagementSystem.PublicApi.PurchaseOrderEndpoint
{
    public class PurchaseOrderRequest : BaseRequest
    {
        public string SupplierID { get; set; }
        public DateTime DeliveryTime { get; set; }
        
        public List<Product> Products { get; set; }
    }
}