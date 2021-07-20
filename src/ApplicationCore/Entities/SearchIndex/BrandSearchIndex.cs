using System;
using System.Collections.Generic;

namespace InventoryManagementSystem.ApplicationCore.Entities.SearchIndex
{
    public class BrandSearchIndex : BaseSearchIndex
    {
        public string BrandId { get; set; }
        public string BrandName { get; set; }
        public List<BrandProductIndexInfo> BrandProductIndexInfos { get; set; } = new List<BrandProductIndexInfo>();
    }

    public class BrandProductIndexInfo
    {
        public string ProductId { get; set; }
        public string ProductName { get; set; }
    }

}