using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using InventoryManagementSystem.ApplicationCore.Entities.Orders.Status;

namespace InventoryManagementSystem.ApplicationCore.Entities.Orders
{
    public class Transaction : BaseEntity
    {
        public Transaction()
        {
            Id = DateTime.UtcNow.Date.ToString("ddMMyyyy") + "-"+Guid.NewGuid();
        }
        
        public TransactionType CurrentType { get; set; }
        public bool TransactionStatus { get; set; }
        public virtual IList<TransactionRecord> TransactionRecord { get; set; }
        
        [NotMapped]
        public IList<TransactionRecordCompact> TransactionRecordCompacts { get; set; }

        // [OnSerialized]
        // public void FormatTransactionResponse(StreamingContext context)
        // {
        //     TransactionRecord = TransactionRecord.GroupBy(e => e.UserTransactionActionType).Select(g => g.LastOrDefault()).ToList();
        // }
        
        [OnSerializing]
        public void FillDataOfTransactionRecordCompact(StreamingContext context)
        {
            TransactionRecordCompacts = new List<TransactionRecordCompact>();
            foreach (var transactionRecord in TransactionRecord)
            {
                TransactionRecordCompacts.Add(new TransactionRecordCompact()
                {
                    User = transactionRecord.ApplicationUser.Fullname,
                    Action = transactionRecord.UserTransactionActionType.ToString(),
                    Date = transactionRecord.Date,
                    TransactionName = transactionRecord.Name,
                    Type = transactionRecord.Type.ToString()
                });
            }
        }
    }
    
    public class TransactionRecordCompact
    {
        public string TransactionName { get; set; }
        public string User { get; set; }
        public string Type { get; set; }
        public DateTime Date { get; set; }
        public string Action { get; set; }
    }
}