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
        
        //Dictionary<Packge, Number of product to get> in FIFO pattern
        public async Task<Dictionary<string, int>> FifoPackagesSuggestion(List<OrderItem> orderItems)
        {
            Dictionary<string, int> numProductInPackageFIFO = new Dictionary<string, int>();
            List<Package> packages = new List<Package>();
            foreach (var orderItem in orderItems)
            {
                packages = (await _packageAsyncRepository.ListAllAsync(new PagingOption<Package>(0, 0))).ResultList
                    .Where(package => package.ProductVariantId == orderItem.ProductVariantId).OrderBy(package => package.ImportedDate).ToList();
                var temPackages = new List<Package>(packages);
                var quantityToDeduce = orderItem.OrderQuantity;
                
                for (var i = 0; i < temPackages.Count; i++)
                {
                    if (quantityToDeduce > 0)
                    {
                        if (temPackages[i].Quantity >= quantityToDeduce)
                        {
                            if (numProductInPackageFIFO.ContainsKey(packages[i].Id))
                                numProductInPackageFIFO[packages[i].Id] = quantityToDeduce;
                            else
                                numProductInPackageFIFO.Add(packages[i].Id, quantityToDeduce);
                        }
                        
                        else
                        {
                            quantityToDeduce -= temPackages[i].Quantity;
                            if (numProductInPackageFIFO.ContainsKey(packages[i].Id))
                                numProductInPackageFIFO[packages[i].Id] = temPackages[i].Quantity;
                            else
                              numProductInPackageFIFO.Add(packages[i].Id, temPackages[i].Quantity);
                        }
                    }
                }
            }
            return numProductInPackageFIFO;
        }
    }
}