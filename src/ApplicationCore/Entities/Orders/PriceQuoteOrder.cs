﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using InventoryManagementSystem.ApplicationCore.Entities.RequestAndForm;

namespace InventoryManagementSystem.ApplicationCore.Entities.Orders
{
    public class PriceQuoteOrder : BaseEntity
    {
        public PriceQuoteOrder()
        {
            Id = DateTime.UtcNow + "-"+Guid.NewGuid().ToString();
            PriceQuoteOrderNumber = DateTime.UtcNow.Date.ToString("yyyyMMdd") +
                                    Guid.NewGuid().ToString().Substring(0, 5).ToUpper();
            CreatedDate = DateTime.Now;
            PriceQuoteStatus = PriceQuoteType.Pending;
            Type = TransactionType.PriceQuote;
        }

        public string PriceQuoteOrderNumber { get; set; }
        public string SupplierId { get; set; }
        public virtual Supplier Supplier { get; set; }
        
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        
        public DateTime Deadline { get; set; }

        public string Description { get; set; }

        public string WarehouseLocation { get; set; }
        
        public string CreatedById { get; set; }

        public PriceQuoteType PriceQuoteStatus { get; set; }
        public TransactionType Type { get; set; }

        public virtual ICollection<OrderItemInfo> PurchaseOrderProduct { get; set; } = new List<OrderItemInfo>();
        public virtual UserInfo CreatedBy { get; set; }
        
    }
}