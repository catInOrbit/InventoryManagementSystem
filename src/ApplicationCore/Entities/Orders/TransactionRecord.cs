using System;
using InventoryManagementSystem.ApplicationCore.Entities.Orders.Status;

namespace InventoryManagementSystem.ApplicationCore.Entities.Orders
{
    public class TransactionRecord : BaseEntity
    {
        public TransactionRecord()
        {
            Id = DateTime.UtcNow.Date.ToString("ddMMyyyy") + "-"+Guid.NewGuid();
        }

        public DateTime Date { get; set; }
        public string TransactionId { get; set; }
        public virtual Transaction Transaction { get; set; }
        public string OrderId { get; set; }
        public string Name { get; set; }
        public string ApplicationUserId { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
        public UserTransactionActionType UserTransactionActionType { get; set; }
    }
}