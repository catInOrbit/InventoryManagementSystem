using System;
using System.Collections;
using System.Collections.Generic;
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
        public string ApplicationUserId { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
        public UserTransactionActionType UserTransactionActionType { get; set; }
        public TransactionType Type { get; set; }
        public bool TransactionStatus { get; set; }
        
        public virtual IList<TransactionRecord> TransactionRecord { get; set; }
    }
}