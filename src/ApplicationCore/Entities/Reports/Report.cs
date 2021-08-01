using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace InventoryManagementSystem.ApplicationCore.Entities.Reports
{
    public class StockOnhandReport : BaseEntity
    {
        [JsonIgnore]
        public override string Id { get; set; }

        public StockOnhandReport()
        {
            Id = Guid.NewGuid().ToString();
        }
        public string ProductVariantId { get; set; }
        public string ProductVariantName { get; set; }
        
        public DateTime CreatedDate { get; set; }
        public int StorageQuantity { get; set; }
        [Column(TypeName = "decimal(16,3)")]
        public decimal Value { get; set; }

        [JsonIgnore]
        public bool DoesNotHavePackageInfo { get; set; }

        public bool ShouldSerializeCreatedDate()
        {
            if (DoesNotHavePackageInfo)
                return true;
            return false;
        }
        
        public bool ShouldSerializeStorageQuantity()
        {
            if (DoesNotHavePackageInfo)
                return true;
            return false;
        }
        
        
        public bool ShouldSerializeValue()
        {
            if (DoesNotHavePackageInfo)
                return true;
            return false;
        }

        public List<StockImportInfo> StockImportPackageInfos { get; set; } = new List<StockImportInfo>();
    }

    public class StockImportInfo
    {
        public DateTime Date { get; set; }
        public int StorageQuantity { get; set; }
        [Column(TypeName = "decimal(16,3)")]
        public decimal Value { get; set; }
    }
    
    public class StockDefaultInfo
    {
        public DateTime CreatedDate { get; set; }
        public int StorageQuantity { get; set; }
        [Column(TypeName = "decimal(16,3)")]
        public decimal Value { get; set; }
    }
    
    public class StockTakeReport : BaseEntity
    {
        [JsonIgnore]
        public override string Id { get; set; }

        public StockTakeReport()
        {
            Id = Guid.NewGuid().ToString();
        }
        
        public DateTime StockTakeDate { get; set; }
        public string ProductName { get; set; }
        public int StorageQuantity { get; set; }
        public int ActualQuantity { get; set; }
        // [Column(TypeName = "decimal(16,3)")]
        // public decimal Value { get; set; }
    }

    public class TopSellingReport : BaseEntity
    {
        [JsonIgnore]
        public override string Id { get; set; }

        public TopSellingReport()
        {
            Id = Guid.NewGuid().ToString();
        }
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public int TotalSold { get; set; }
        public string ReportDate { get; set; }
        public string ReportType { get; set; }
    }

    public enum ReportType
    {
        Month= 1,
        Year = 2,
    }
    
}