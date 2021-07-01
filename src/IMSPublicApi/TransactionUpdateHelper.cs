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
                Name = actionName,
                Type = transactionType,
                ApplicationUserId = userId,
                TransactionStatus = true,
                UserTransactionActionType = UserTransactionActionType.Create,
                TransactionRecord = new List<TransactionRecord>()
            };
            
            transaction.TransactionRecord.Add( new 
                TransactionRecord
            {
                Date = DateTime.Now,
                Transaction = transaction,
                OrderId = objectId,
                TransactionId = transaction.Id
            });

            return transaction;
        }
        
        
        public static Transaction UpdateTransaction(Transaction transaction, UserTransactionActionType userTransactionActionType, string objectId, string userId)
        {
            var latestRecord = transaction.TransactionRecord[^1];
            
            string actionName = String.Format("Update {0}, ID: {1}",latestRecord.Transaction.Type.ToString(),objectId);

            transaction.Name = actionName;
            transaction.ApplicationUserId = userId;
            transaction.UserTransactionActionType = userTransactionActionType;
            
            transaction.TransactionRecord.Add(new 
                TransactionRecord
                {
                    Date = DateTime.Now,
                    Transaction = transaction,
                    OrderId = objectId,
                    TransactionId = transaction.Id
                });
            
            return transaction;
        }
        
        
        
        
    }
}