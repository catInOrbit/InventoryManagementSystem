using Microsoft.eShopWeb.ApplicationCore.Interfaces;

namespace Microsoft.eShopWeb.ApplicationCore.Entities
{
    public class ProductBrand : BaseEntity
    {
        public string Brand { get; private set; }
        public ProductBrand(string brand)
        {
            Brand = brand;
        }
    }
}
