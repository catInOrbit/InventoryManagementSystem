using System;

namespace InventoryManagementSystem.ApplicationCore.Entities.Orders
{
    public class TransactionRecord : BaseEntity
    {
        public TransactionRecord()
        {
            Id = DateTime.Now.Date.ToString("ddMMyyyy") + "-"+Guid.NewGuid();
        }

        public DateTime Date { get; set; }
        public string TransactionId { get; set; }
        public virtual Transaction Transaction { get; set; }
        public string OrderId { get; set; }
    }
}