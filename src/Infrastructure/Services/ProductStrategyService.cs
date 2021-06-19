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


        public async Task<List<Package>> FIFOPackagesSuggestion(IList<string> productVariantIds)
        {
            List<Package> packages = new List<Package>();
            var pagingOption = await _packageAsyncRepository.ListAllAsync(new PagingOption<Package>(0, 0));

            packages.AddRange(pagingOption.ResultList.Where(package => productVariantIds.Contains(package.ProductVariantId)));
            return packages;
        }

     
    }
}