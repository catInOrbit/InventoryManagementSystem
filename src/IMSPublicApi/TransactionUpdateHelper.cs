using System;
using System.Collections.Generic;
using InventoryManagementSystem.ApplicationCore.Entities.Orders;
using InventoryManagementSystem.ApplicationCore.Entities.Orders.Status;

namespace InventoryManagementSystem.PublicApi
{
    public class TransactionUpdateHelper 
    {
        public static Transaction CreateNewTransaction(TransactionType transactionType, string objectId, string userId)
        {
            string actionName = String.Format("Created new {0}, ID: {1}",transactionType.ToString(),objectId);

            var transaction = new Transaction
            {
                Type = transactionType,
                TransactionStatus = true,
                TransactionRecord = new List<TransactionRecord>()
            };
            
            transaction.TransactionRecord.Add( new 
                TransactionRecord
            {
                Date = DateTime.UtcNow,
                Transaction = transaction,
                OrderId = objectId,
                TransactionId = transaction.Id,
                Name = actionName,
                ApplicationUserId =  userId,
                UserTransactionActionType = UserTransactionActionType.Create
            });

            return transaction;
        }
        
        
        public static Transaction UpdateTransaction(Transaction transaction,
            UserTransactionActionType userTransactionActionType, string userId, string objectId, string reason)
        {
            var latestRecord = transaction.TransactionRecord[^1];
            
            
            string actionName = String.Format("{0} {1}, ID: {2}",userTransactionActionType.ToString() ,latestRecord.Transaction.Type.ToString(),objectId);
            if (userTransactionActionType == UserTransactionActionType.Reject)
                actionName += ".Reason: " + reason;
            
            transaction.TransactionRecord.Add(new 
                TransactionRecord
                {
                    Date = DateTime.UtcNow,
                    Transaction = transaction,
                    OrderId = objectId,
                    TransactionId = transaction.Id,
                    Name = actionName,
                    ApplicationUserId = userId,
                    UserTransactionActionType = userTransactionActionType
                });
            
            return transaction;
        }
        
        
        
        
    }
}