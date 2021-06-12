// using System;
// using System.Collections.Generic;
// using System.ComponentModel.DataAnnotations;
// using System.ComponentModel.DataAnnotations.Schema;
// using System.Text.Json.Serialization;
// using InventoryManagementSystem.ApplicationCore.Entities.Orders.Status;
// using InventoryManagementSystem.ApplicationCore.Entities.RequestAndForm;
//
// namespace InventoryManagementSystem.ApplicationCore.Entities.Orders
// {
//     public class PriceQuoteOrder : BaseEntity
//     {
//         public PriceQuoteOrder()
//         {
//             Id = DateTime.Now.Date.ToString("ddMMyyyy") + "-"+Guid.NewGuid();
//             // Transaction.TransactionId = Id;
//             // Transaction.TransactionNumber = DateTime.UtcNow.Date.ToString("ddMMyyyy") +
//             //                                    Guid.NewGuid().ToString().Substring(0, 5).ToUpper();
//             // Transaction.CreatedDate = DateTime.Now;
//             PriceQuoteStatus = PriceQuoteType.Pending;
//             // Transaction.Type = TransactionType.PriceQuote;
//             PriceQuoteNumber = DateTime.UtcNow.Date.ToString("ddMMyyyy") +
//                                Guid.NewGuid().ToString().Substring(0, 5).ToUpper();
//         }
//         
//         public string PriceQuoteNumber { get; set; }
//         public string SupplierId { get; set; }
//         public virtual Supplier Supplier { get; set; }
//         public DateTime Deadline { get; set; }
//         public string MailDescription { get; set; }
//         public PriceQuoteType PriceQuoteStatus { get; set; }
//         public decimal TotalOrderAmount { get; set; }
//         public virtual ICollection<OrderItem> PurchaseOrderProduct { get; set; } = new List<OrderItem>();
//         public virtual Transaction Transaction { get; set; }
//         
//     }
// }