using System;
using System.Collections;
using System.Collections.Generic;
using Ardalis.ApiEndpoints;
using InventoryManagementSystem.ApplicationCore.Entities.Orders;

namespace InventoryManagementSystem.PublicApi.PurchaseOrderEndpoint.PriceQuote
{
    public class PQEditRequest : BaseRequest 
    {
        // public PriceQuoteOrder PriceQuoteOrder { get; set; }
        
        public string PurchaseOrderNumber { get; set; }
        public string SupplierId { get; set; }
        public DateTime Deadline { get; set; }
        public string MailDescription { get; set; }
        public ICollection<OrderItem> OrderItemInfos  { get; set; }
    }
}