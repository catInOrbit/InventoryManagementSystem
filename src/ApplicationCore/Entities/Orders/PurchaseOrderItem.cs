﻿using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using InventoryManagementSystem.ApplicationCore.Entities.Products;

namespace InventoryManagementSystem.ApplicationCore.Entities.Orders
{
    public class PurchaseOrderItem : BaseEntity
    {
        public PurchaseOrderItem()
        {
            Id = Guid.NewGuid().ToString();
        }
        [JsonIgnore]
        public string OrderNumber { get; set; }
        public string ProductVariantId { get; set; }
        [JsonIgnore]
        public virtual ProductVariant ProductVariant { get; set; }
        public float OrderQuantity { get; set; }
        public string Unit { get; set; }
        public decimal Price { get; set; }
        public decimal DiscountAmount { get; set; }
        
        [JsonIgnore]
        public decimal TotalAmount { get; set; }
    }
}