﻿using System;
using System.Text.Json.Serialization;
using InventoryManagementSystem.ApplicationCore.Entities.Orders.Status;
using InventoryManagementSystem.ApplicationCore.Entities.Products;

namespace InventoryManagementSystem.ApplicationCore.Entities.Orders
{
    public class Transaction : BaseEntity
    {
        public Transaction()
        {
            Id = DateTime.Now.Date.ToString("ddMMyyyy") + "-"+Guid.NewGuid();
        }
        
        public string Name { get; set; }
        public  DateTime ValidUntil { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string CreatedById { get; set; }
        public virtual ApplicationUser CreatedBy { get; set; }
        public string ConfirmedById { get; set; }
        public virtual ApplicationUser ConfirmedBy { get; set; }
        public string ModifiedById { get; set; }
        public virtual ApplicationUser ModifiedBy { get; set; }

        public TransactionType Type { get; set; }
        public bool TransactionStatus { get; set; }
    }
}