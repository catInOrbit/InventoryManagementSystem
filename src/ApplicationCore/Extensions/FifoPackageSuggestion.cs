using System.Collections.Generic;
using System.Runtime.Serialization;
using InventoryManagementSystem.ApplicationCore.Entities.Orders;
using InventoryManagementSystem.ApplicationCore.Entities.Products;

namespace InventoryManagementSystem.ApplicationCore.Extensions
{
    public class FifoPackageSuggestion
    {
        public OrderItem OrderItem { get; set; }
        public List<PackageAndQuantity> PackagesAndQuantitiesToGet { get; set; } = new List<PackageAndQuantity>();
        
        [OnSerializing]
        public void FormatJsonResponse(StreamingContext context)
        {
            OrderItem.IsShowingProductVariant = true;
            OrderItem.IsShowingProductVariantDetail = false;
            foreach (var packagesAndQuantity in PackagesAndQuantitiesToGet)
            {
                packagesAndQuantity.PackageToGet.IsDisplayingDetail = false;
                packagesAndQuantity.PackageToGet.IsShowingProductVariant = false;
            }
        }
    }

    public class PackageAndQuantity
    {
        public Package PackageToGet { get; set; }
        public int QuantityToGet { get; set; }
    }
}