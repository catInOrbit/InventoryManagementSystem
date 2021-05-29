using System;
using InventoryManagementSystem.ApplicationCore.Entities.Orders;

namespace InventoryManagementSystem.ApplicationCore.Entities.RequestAndForm
{
    public class Transaction : BaseEntity
    {
        public  string Name { get; set; }
        public  DateTime ValidUntil { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string ConfirmedBy { get; set; }
        public string SupplierId { get; set; }
        public DateTime DeliveryTime { get; set; }
        public DateTime TrackingNumber { get; set; }
        public virtual Supplier Supplier { get; set; }
        public bool Status { get; set; }
    }
}