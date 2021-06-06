using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using InventoryManagementSystem.ApplicationCore.Entities.Orders;
using InventoryManagementSystem.ApplicationCore.Entities.Orders.Status;

namespace InventoryManagementSystem.ApplicationCore.Entities.RequestAndForm
{
    public class Transaction : BaseEntity
    {
        public string Name { get; set; }
        public string TransactionId { get; set; }
        public  DateTime ValidUntil { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string CreatedById { get; set; }
        public string ConfirmedBy { get; set; }
        public DateTime TrackingNumber { get; set; }
        public TransactionType Type { get; set; }
        public bool Status { get; set; }
    }
}