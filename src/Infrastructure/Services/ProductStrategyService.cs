using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InventoryManagementSystem.ApplicationCore.Entities;
using InventoryManagementSystem.ApplicationCore.Entities.Orders;
using InventoryManagementSystem.ApplicationCore.Entities.Products;
using InventoryManagementSystem.ApplicationCore.Interfaces;

namespace Infrastructure.Services
{
    public class ProductStrategyService
    {
        private IAsyncRepository<Package> _packageAsyncRepository;

        public ProductStrategyService(IAsyncRepository<Package> packageAsyncRepository)
        {
            _packageAsyncRepository = packageAsyncRepository;
        }
        
        //Dictionary<Number of product to get, in list of packages> in FIFO pattern
        public async Task<Dictionary<int, Package>> FifoPackagesSuggestion(List<OrderItem> orderItems)
        {
            Dictionary<int, Package> numProductInPackageFIFO = new Dictionary<int, Package>();
            List<Package> packages = new List<Package>();
            foreach (var orderItem in orderItems)
            {
                packages = (await _packageAsyncRepository.ListAllAsync(new PagingOption<Package>(0, 0))).ResultList
                    .Where(package => package.ProductVariantId == orderItem.ProductVariantId).ToList();
                var quantityToDeduce = orderItem.OrderQuantity;
                
                for (var i = 0; i < packages.Count; i++)
                {
                    if (quantityToDeduce > 0)
                    {
                        if (packages[i].Quantity >= quantityToDeduce)
                        {
                            packages[i].Quantity -= quantityToDeduce;
                        
                            //Remove aggregated quantity of product as well
                            numProductInPackageFIFO.Add(quantityToDeduce, packages[i]);
                        }
                        
                        else
                        {
                            quantityToDeduce -= packages[i].Quantity;
                            packages[i].Quantity -= packages[i].Quantity;
                        
                            numProductInPackageFIFO.Add(packages[i].Quantity, packages[i]);
                        }
                    }
                }
            }
            return numProductInPackageFIFO;
        }
    }
}