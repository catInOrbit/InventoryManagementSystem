﻿using System;
using System.Collections.Generic;
using InventoryManagementSystem.ApplicationCore.Entities.Orders.Status;

namespace InventoryManagementSystem.ApplicationCore.Entities.Orders
{
    public class PurchaseOrder : BaseEntity
    {
        public PurchaseOrder()
        {
            Id = "PO" +Guid.NewGuid().ToString().Substring(0, 8).ToUpper();
            // Transaction.TransactionId = Id;
            // Transaction.TransactionNumber =  DateTime.UtcNow.Date.ToString("ddMMyyyy") +
            //                Guid.NewGuid().ToString().Substring(0, 5).ToUpper();
            PurchaseOrderStatus = PurchaseOrderStatusType.PQCreated;
            // Transaction.CreatedDate = DateTime.Now;
            // Transaction.Type = TransactionType.Purchase;
        }
        public virtual bool ShouldSerializePurchaseOrderProduct()
        {
            return true;
        }
        
        public DateTime DeliveryDate { get; set; }
        public string DeliveryAddress { get; set; }
        public string MailDescription { get; set; }
        public string SupplierId { get; set; }
        public virtual Supplier Supplier { get; set; }
        public string WarehouseLocation { get; set; }
        public PurchaseOrderStatusType PurchaseOrderStatus { get; set; }
        public decimal TotalDiscountAmount { get; set; }
        public decimal TotalOrderAmount { get; set; }
        public virtual ICollection<OrderItem> PurchaseOrderProduct { get; set; } = new List<OrderItem>();
        public string TransactionId { get; set; }
        public DateTime Deadline { get; set; }
        public virtual Transaction Transaction { get; set; }
        
    }
}
