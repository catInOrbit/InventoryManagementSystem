using System.Collections.Generic;
using System.Runtime.Serialization;
using InventoryManagementSystem.ApplicationCore.Entities.Products;

namespace InventoryManagementSystem.ApplicationCore.Extensions
{
    public class FifoPackageSuggestion
    {
        public ProductVariant ProductVariant { get; set; }
        public List<PackageAndQuantity> PackagesAndQuantities { get; set; } = new List<PackageAndQuantity>();
        
        [OnSerializing]
        public void FormatJsonResponse(StreamingContext context)
        {
            ProductVariant.IsShowingPackage = false;
            ProductVariant.IsShowingTransaction = false;
            
            foreach (var packagesAndQuantity in PackagesAndQuantities)
            {
                packagesAndQuantity.Package.IsDisplayingDetail = false;
                packagesAndQuantity.Package.IsShowingProductVariant = false;

            }
        }
    }

    public class PackageAndQuantity
    {
        public Package Package { get; set; }
        public int Quantity { get; set; }
    }
}