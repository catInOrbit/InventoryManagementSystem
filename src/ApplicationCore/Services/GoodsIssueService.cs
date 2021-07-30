using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InventoryManagementSystem.ApplicationCore.Constants;
using InventoryManagementSystem.ApplicationCore.Entities;
using InventoryManagementSystem.ApplicationCore.Entities.Orders;
using InventoryManagementSystem.ApplicationCore.Entities.Products;
using InventoryManagementSystem.ApplicationCore.Interfaces;

namespace InventoryManagementSystem.ApplicationCore.Services
{
    public class GoodsIssueService
    {
        private readonly IAsyncRepository<Package> _packageAsyncRepository;
        
        private readonly IAsyncRepository<GoodsIssueOrder> _gioAsyncRepository;
        private readonly IAsyncRepository<ProductVariant> _productVariantAsyncRepository;
        private readonly IElasticAsyncRepository<Package> _packageIndexAsyncRepository;

        public GoodsIssueService(IAsyncRepository<Package> packageAsyncRepository)
        {
            _packageAsyncRepository = packageAsyncRepository;
        }

        public GoodsIssueService(IAsyncRepository<GoodsIssueOrder> gioAsyncRepository, IAsyncRepository<ProductVariant> productVariantAsyncRepository, IAsyncRepository<Package> packageAsyncRepository, IElasticAsyncRepository<Package> packageIndexAsyncRepository)
        {
            _gioAsyncRepository = gioAsyncRepository;
            _productVariantAsyncRepository = productVariantAsyncRepository;
            _packageAsyncRepository = packageAsyncRepository;
            _packageIndexAsyncRepository = packageIndexAsyncRepository;
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

        public async Task UpdatePackageFromGoodsIssue(GoodsIssueOrder gio)
        {
            foreach (var gioGoodsIssueProduct in gio.GoodsIssueProducts)
            {
                var listPackages =
                    await _gioAsyncRepository.GetPackagesFromProductVariantId(gioGoodsIssueProduct.ProductVariantId);
                var productVariant =
                    await _productVariantAsyncRepository.GetByIdAsync(gioGoodsIssueProduct.ProductVariantId);
                List<int> listIndexPackageToRemove = new List<int>();
                var quantityToDeduce = gioGoodsIssueProduct.OrderQuantity;
                for (var i = 0; i < listPackages.Count; i++)
                {
                    if (quantityToDeduce > 0)
                    {
                        if (listPackages[i].Quantity >= quantityToDeduce)
                        {
                            listPackages[i].Quantity -= quantityToDeduce;
                            listPackages[i].LatestUpdateDate = DateTime.UtcNow;
                            //Remove aggregated quantity of product as well
                            productVariant.StorageQuantity -= quantityToDeduce; 
                        }
                        else
                        {
                            quantityToDeduce -= listPackages[i].Quantity;
                            listPackages[i].Quantity -= listPackages[i].Quantity;
                        
                            //Remove aggregated quantity of product as well
                            productVariant.StorageQuantity -= quantityToDeduce; 
                        }
                    }
        
                    if (listPackages[i].Quantity <= 0)
                        listIndexPackageToRemove.Add(i);
                }
        
                await _productVariantAsyncRepository.UpdateAsync(productVariant);

                foreach (var i in listIndexPackageToRemove)
                {
                    await _packageIndexAsyncRepository.ElasticDeleteSingleAsync(listPackages[i], ElasticIndexConstant.PACKAGES);
                    await _packageAsyncRepository.DeleteAsync(listPackages[i]);
                }
            }
        }
    }
}