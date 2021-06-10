using System.Collections;
using System.Collections.Generic;
using InventoryManagementSystem.ApplicationCore.Entities.Products;

namespace InventoryManagementSystem.PublicApi.ProductEndpoints.Create
{
    public class ProductCreateRequest : BaseRequest
    {
        public string  ProductName { get; set; }
        public string  Barcode { get; set; }
        public string  CategoryId { get; set; }
        public string  BrandName { get; set; }
        public string  Unit { get; set; }
        public bool IsVariantType { get; set; }
        public List<ProductVariant> ProductVariants { get; set; } = new List<ProductVariant>();
        public string SellingStrategy;
    }
}