using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using InventoryManagementSystem.ApplicationCore.Entities.Orders.Status;
using InventoryManagementSystem.ApplicationCore.Entities.Products;
using Newtonsoft.Json;

namespace InventoryManagementSystem.ApplicationCore.Entities.Orders
{
    public class Transaction : BaseEntity
    {
        public Transaction()
        {
            Id = DateTime.UtcNow.Date.ToString("ddMMyyyy") + "-"+Guid.NewGuid();
        }
        public TransactionType Type { get; set; }
        public bool TransactionStatus { get; set; }
        
        public virtual IList<TransactionRecord> TransactionRecord { get; set; }

        [OnSerialized]
        public void FormatTransactionResponse(StreamingContext context)
        {
            TransactionRecord = TransactionRecord.GroupBy(e => e.UserTransactionActionType).Select(g => g.LastOrDefault()).ToList();
        }
    }
}