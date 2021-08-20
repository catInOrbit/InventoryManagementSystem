using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InventoryManagementSystem.ApplicationCore.Constants;
using InventoryManagementSystem.ApplicationCore.Entities;
using InventoryManagementSystem.ApplicationCore.Entities.Orders;
using InventoryManagementSystem.ApplicationCore.Entities.Orders.Status;
using InventoryManagementSystem.ApplicationCore.Entities.Products;
using InventoryManagementSystem.ApplicationCore.Extensions;
using InventoryManagementSystem.ApplicationCore.Interfaces;

namespace InventoryManagementSystem.ApplicationCore.Services
{
    public class GoodsIssueBusinessService
    {
        private readonly IAsyncRepository<Package> _packageAsyncRepository;
        
        private readonly IAsyncRepository<GoodsIssueOrder> _gioAsyncRepository;
        private readonly IAsyncRepository<ProductVariant> _productVariantAsyncRepository;
        private readonly IElasticAsyncRepository<Package> _packageIndexAsyncRepository;

        public GoodsIssueBusinessService(IAsyncRepository<Package> packageAsyncRepository)
        {
            _packageAsyncRepository = packageAsyncRepository;
        }

        public GoodsIssueBusinessService(IAsyncRepository<GoodsIssueOrder> gioAsyncRepository, IAsyncRepository<ProductVariant> productVariantAsyncRepository, IAsyncRepository<Package> packageAsyncRepository, IElasticAsyncRepository<Package> packageIndexAsyncRepository)
        {
            _gioAsyncRepository = gioAsyncRepository;
            _productVariantAsyncRepository = productVariantAsyncRepository;
            _packageAsyncRepository = packageAsyncRepository;
            _packageIndexAsyncRepository = packageIndexAsyncRepository;
        }
        
        public string ValidateGoodsIssue(GoodsIssueOrder gi)
        {
            var productVariantIdList = new List<string>();
            foreach (var giGoodsIssueProduct in gi.GoodsIssueProducts)
            {
                productVariantIdList.Add(giGoodsIssueProduct.ProductVariantId);
                // if (string.IsNullOrEmpty(giGoodsIssueProduct.ProductVariant.Sku))
                //     return String.Format("Detected a product variant with empty sku: {0}, please update all products", giGoodsIssueProduct.ProductVariant.Id);
            }

            if (productVariantIdList.Count != productVariantIdList.Distinct().Count())
                return "Duplicate product found in issue order";
            return null;
        }

        
        public async Task<List<FifoPackageSuggestion>> FifoPackageCalculate(List<OrderItem> orderItems)
        {
            Dictionary<string, int> numProductInPackageFIFO = new Dictionary<string, int>();
            Dictionary<string, int> ProductAndOrderAmount = new Dictionary<string, int>();
            
            List<FifoPackageSuggestion> suggestions = new List<FifoPackageSuggestion>(); 
            List<Package> packages = new List<Package>();
            
            //Assume order item has no duplicate productVariantId
            foreach (var orderItem in orderItems)
            {
                ProductAndOrderAmount.Add(orderItem.ProductVariantId, orderItem.OrderQuantity);

                // var key = ProductAndOrderAmount.Keys.FirstOrDefault(k => k == orderItem.ProductVariantId);
                // // if (key != null)
                //     // ProductAndOrderAmount[key] += orderItem.OrderQuantity;
                // if(key != null && ProductAndOrderAmount.ContainsKey(key))
                //     ProductAndOrderAmount[key] += orderItem.OrderQuantity;
                // else
                // {
                //     ProductAndOrderAmount.Add(orderItem.ProductVariantId, orderItem.OrderQuantity);
                // }
            }
            
            foreach (var productAndOrder in ProductAndOrderAmount)
            {
                packages = (await _packageAsyncRepository.ListAllAsync(new PagingOption<Package>(0, 0))).ResultList
                    .Where(package => package.ProductVariantId == productAndOrder.Key).OrderBy(package => package.ImportedDate).ToList();
                var temPackages = new List<Package>(packages);
                var quantityToDeduce = productAndOrder.Value; // 80
                
                var fifopackage = new FifoPackageSuggestion();
                fifopackage.OrderItem = orderItems.FirstOrDefault(o => o.ProductVariantId == productAndOrder.Key);
                foreach (var package in packages) // 2 packages: 1 50 2 60
                {
                    if(quantityToDeduce <= 0 )
                        break;
                    if (package.Quantity > quantityToDeduce)
                    {
                        fifopackage.PackagesAndQuantitiesToGet.Add(new PackageAndQuantity
                        {
                            PackageToGet = package,
                            QuantityToGet = quantityToDeduce
                        });
                        
                        quantityToDeduce = 0;
                    }

                    else
                    {
                        quantityToDeduce -= package.Quantity;
                        fifopackage.PackagesAndQuantitiesToGet.Add(new PackageAndQuantity
                        {
                            PackageToGet = package,
                            QuantityToGet = package.Quantity
                        });
                    }
                    
                    suggestions.Add(fifopackage);
                }
                
                // //
                // // for (var i = 0; i < temPackages.Count; i++)
                // // {
                // //     if (quantityToDeduce > 0)
                // //     {
                // //         if (temPackages[i].Quantity >= quantityToDeduce)
                // //         {
                // //             if (numProductInPackageFIFO.ContainsKey(packages[i].Id))
                // //                 numProductInPackageFIFO[packages[i].Id] = quantityToDeduce;
                // //             else
                // //                 numProductInPackageFIFO.Add(packages[i].Id, quantityToDeduce);
                // //         }
                // //         
                // //         else
                // //         {
                // //             quantityToDeduce -= temPackages[i].Quantity;
                // //             if (numProductInPackageFIFO.ContainsKey(packages[i].Id))
                // //                 numProductInPackageFIFO[packages[i].Id] = temPackages[i].Quantity;
                // //             else
                // //                 numProductInPackageFIFO.Add(packages[i].Id, temPackages[i].Quantity);
                // //         }
                // //     }
                // // }
                // //
                // var fifopackage = new FifoPackageSuggestion();
                // fifopackage.OrderItem = orderItems.FirstOrDefault(o => o.ProductVariantId == productAndOrder.Key);
                // foreach (var keyValuePair in numProductInPackageFIFO)
                // {
                //     var package = (await _packageAsyncRepository.GetByIdAsync(keyValuePair.Key));
                //     if (fifopackage.OrderItem.ProductVariant.Packages.Contains(package))
                //     {
                //         fifopackage.PackagesAndQuantitiesToGet.Add(new PackageAndQuantity
                //         {
                //             PackageToGet = package,
                //             QuantityToGet = keyValuePair.Value
                //         });
                //     }
                // }
                // suggestions.Add(fifopackage);
            }
            //
            // foreach (var orderItem in orderItems)
            // {
            //     var fifopackage = new FifoPackageSuggestion();
            //     fifopackage.ProductVariant = orderItem.ProductVariant;
            //     foreach (var keyValuePair in numProductInPackageFIFO)
            //     {
            //         var package = (await _packageAsyncRepository.GetByIdAsync(keyValuePair.Key));
            //         if (fifopackage.ProductVariant.Packages.Contains(package))
            //         {
            //             fifopackage.PackagesAndQuantities.Add(new PackageAndQuantity
            //             {
            //                 Package = package,
            //                 Quantity = keyValuePair.Value
            //             });
            //         }
            //     }
            //     suggestions.Add(fifopackage);
            // }
            
            return suggestions;
        }
        public async Task UpdatePackageFromGoodsIssue(GoodsIssueOrder gio, string userId)
        {
            foreach (var gioGoodsIssueProduct in gio.GoodsIssueProducts)
            {
                var listPackages =
                    await _gioAsyncRepository.GetPackagesFromProductVariantId(gioGoodsIssueProduct.ProductVariantId);
                var productVariant =
                    await _productVariantAsyncRepository.GetByIdAsync(gioGoodsIssueProduct.ProductVariantId);
                
                List<int> listIndexPackageToRemove = new List<int>();
                var quantityToDeduce = gioGoodsIssueProduct.OrderQuantity;
                productVariant.StorageQuantity =0;

                for (var i = 0; i < listPackages.Count; i++)
                {
                    if (quantityToDeduce > 0)
                    {
                        if (listPackages[i].Quantity >= quantityToDeduce)
                        {
                            listPackages[i].Quantity -= quantityToDeduce;
                            listPackages[i].LatestUpdateDate = DateTime.UtcNow;
                            listPackages[i].Transaction = TransactionUpdateHelper.UpdateTransaction(
                                listPackages[i].Transaction, UserTransactionActionType.Modify, TransactionType.Package,
                                userId, listPackages[i].Id, "");
                        }
                        else
                        {
                            quantityToDeduce -= listPackages[i].Quantity;
                            listPackages[i].Quantity -= listPackages[i].Quantity;
                        
                            listPackages[i].Transaction = TransactionUpdateHelper.UpdateTransaction(
                                listPackages[i].Transaction, UserTransactionActionType.Modify, TransactionType.Package,
                                userId, listPackages[i].Id, "");
                        }
                        productVariant.StorageQuantity += listPackages[i].Quantity;
                    }
        
                    if (listPackages[i].Quantity <= 0)
                        listIndexPackageToRemove.Add(i);
                    
                    else
                    {
                        listPackages[i].TotalPrice = listPackages[i].Price * listPackages[i].Quantity;
                        await _packageAsyncRepository.UpdateAsync(listPackages[i]);
                        await _packageIndexAsyncRepository.ElasticSaveSingleAsync(false, listPackages[i], ElasticIndexConstant.PACKAGES);
                    }
                }
        
                await _productVariantAsyncRepository.UpdateAsync(productVariant);

                foreach (var i in listIndexPackageToRemove)
                {
                    listPackages[i].Transaction.TransactionStatus = false;
                    await _packageIndexAsyncRepository.ElasticDeleteSingleAsync(listPackages[i], ElasticIndexConstant.PACKAGES);

                    await _packageAsyncRepository.DeleteAsync(listPackages[i]);
                }
            }
        }
    }
}