using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InventoryManagementSystem.ApplicationCore.Constants;
using InventoryManagementSystem.ApplicationCore.Entities.Orders;
using InventoryManagementSystem.ApplicationCore.Entities.Products;
using InventoryManagementSystem.ApplicationCore.Entities.SearchIndex;
using InventoryManagementSystem.ApplicationCore.Extensions;
using InventoryManagementSystem.ApplicationCore.Interfaces;

namespace InventoryManagementSystem.ApplicationCore.Services
{
    public class GoodsReceiptBusinessService
    {
        private readonly IAsyncRepository<ProductVariant> _productVariantRepository;
        private readonly IAsyncRepository<GoodsReceiptOrder> _roRepository;
        private readonly IAsyncRepository<Package> _packageRepository;

        private readonly IElasticAsyncRepository<ProductVariantSearchIndex> _productVariantEls;
        private readonly IElasticAsyncRepository<GoodsReceiptOrderSearchIndex> _roEls;

        public GoodsReceiptBusinessService(
            IAsyncRepository<ProductVariant> productVariantRepository, 
             IAsyncRepository<GoodsReceiptOrder> roRepository, IAsyncRepository<Package> packageRepository, IElasticAsyncRepository<ProductVariantSearchIndex> productVariantEls, IElasticAsyncRepository<GoodsReceiptOrderSearchIndex> roEls)
        {
            _productVariantRepository = productVariantRepository;
            _roRepository = roRepository;
            _packageRepository = packageRepository;
            _productVariantEls = productVariantEls;
            _roEls = roEls;
        }

        public async Task<GoodsReceiptOrder> ReceiveProducts(GoodsReceiptOrder ro, List<ReceivingOrderUpdateInfo> updateItems)
        {
            ro.ReceivedOrderItems.Clear();
            foreach (var item in updateItems)
            {
                if (item.QuantityReceived > 0)
                {
                    var roi = new GoodsReceiptOrderItem
                    {
                        Id = Guid.NewGuid().ToString(),
                        QuantityReceived =  item.QuantityReceived,
                        ProductVariantId = item.ProductVariantId,
                        GoodsReceiptOrderId = ro.Id,
                        ProductVariantName = (await _productVariantRepository.GetByIdAsync(item.ProductVariantId)).Name
                    };
                    ro.ReceivedOrderItems.Add(roi);
                
                    var productVariant = await _productVariantRepository.GetByIdAsync(roi.ProductVariantId);
                    bool initiateUpdate = false;
                    if (item.Sku != null)
                    {
                        productVariant.Sku = item.Sku;
                        initiateUpdate = true;
                    }

                    if (item.Barcode != null)
                    {
                        productVariant.Barcode = item.Barcode;
                        initiateUpdate = true;
                    }

                    if (initiateUpdate)
                    {
                        await _productVariantRepository.UpdateAsync(productVariant);
                        await _productVariantEls.ElasticSaveSingleAsync(false,
                            IndexingHelper.ProductVariantSearchIndex(productVariant),
                            ElasticIndexConstant.PRODUCT_VARIANT_INDICES);
                    }
                }
            }

            //Update and indexing
            await _roRepository.AddAsync(ro);
            
            foreach (var roi in ro.ReceivedOrderItems)
            {
                //Package
                var package = new Package
                {
                    ProductVariantId =  roi.ProductVariantId,
                    Quantity = roi.QuantityReceived,
                    Location = ro.Location,
                    ImportedDate = ro.ReceivedDate,
                    GoodsReceiptOrderId = ro.Id,
                    Price = ro.PurchaseOrder.PurchaseOrderProduct.FirstOrDefault(item => item.ProductVariantId == roi.ProductVariantId).Price,
                    SupplierId = ro.SupplierId,
                };
                package.TotalPrice = package.Price * package.Quantity;
                package.LatestUpdateDate = DateTime.UtcNow;

                await _packageRepository.AddAsync(package);
                roi.ProductVariant.Packages.Add(package);
                //Begin Inserting into bigQuery
            }
            await _roEls.ElasticSaveSingleAsync(true,IndexingHelper.GoodsReceiptOrderSearchIndex(ro), ElasticIndexConstant.RECEIVING_ORDERS);

            return ro;
        }

    }
}