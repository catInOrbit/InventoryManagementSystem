using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InventoryManagementSystem.ApplicationCore.Constants;
using InventoryManagementSystem.ApplicationCore.Entities.Orders;
using InventoryManagementSystem.ApplicationCore.Entities.Products;
using InventoryManagementSystem.ApplicationCore.Entities.RedisMessages;
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
        private readonly IAsyncRepository<PurchaseOrder> _poAsyncRepository;

        private readonly IElasticAsyncRepository<ProductVariantSearchIndex> _productVariantEls;
        private readonly IElasticAsyncRepository<GoodsReceiptOrderSearchIndex> _roEls;
        private readonly IElasticAsyncRepository<PurchaseOrderSearchIndex> _poEls;

        private readonly IRedisRepository _redisRepository;

        public GoodsReceiptBusinessService(IRedisRepository redisRepository, IAsyncRepository<GoodsReceiptOrder> roRepository, IElasticAsyncRepository<PurchaseOrderSearchIndex> poEls)
        {
            _redisRepository = redisRepository;
            _roRepository = roRepository;
            _poEls = poEls;
        }

        public GoodsReceiptBusinessService(
            IAsyncRepository<ProductVariant> productVariantRepository, 
             IAsyncRepository<GoodsReceiptOrder> roRepository, IAsyncRepository<Package> packageRepository, IElasticAsyncRepository<ProductVariantSearchIndex> productVariantEls, IElasticAsyncRepository<GoodsReceiptOrderSearchIndex> roEls, IAsyncRepository<PurchaseOrder> poAsyncRepository, IRedisRepository redisRepository)
        {
            _productVariantRepository = productVariantRepository;
            _roRepository = roRepository;
            _packageRepository = packageRepository;
            _productVariantEls = productVariantEls;
            _roEls = roEls;
            _poAsyncRepository = poAsyncRepository;
            _redisRepository = redisRepository;
        }
        
        public GoodsReceiptBusinessService(IAsyncRepository<GoodsReceiptOrder> roRepository, IAsyncRepository<PurchaseOrder> poAsyncRepository)
        {
            _roRepository = roRepository;
            _poAsyncRepository = poAsyncRepository;
        }

        public async Task<GoodsReceiptOrder> ReceiveProducts(GoodsReceiptOrder ro, List<ReceivingOrderProductUpdateInfo> updateItems)
        {
            var po = await _poAsyncRepository.GetByIdAsync(ro.PurchaseOrderId);
            foreach (var item in updateItems)
            {
                var orderItem =
                    po.PurchaseOrderProduct.FirstOrDefault(o => o.ProductVariantId == item.ProductVariantId);
                if (item.QuantityReceived >= orderItem.QuantityLeftAfterReceived)
                    orderItem.QuantityLeftAfterReceived = 0;

                else
                    orderItem.QuantityLeftAfterReceived -= item.QuantityReceived;
            }

            await _poAsyncRepository.UpdateAsync(po);
                        
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
                        var skuRedisList = await CheckSkuExistance(po);
                        var skuExist = skuRedisList.FirstOrDefault(s => s.ProductVariantId == productVariant.Id);

                        if(productVariant.Sku!= null && skuExist == null)
                        {
                            productVariant.Sku = item.Sku;
                            initiateUpdate = true;
                        }
                        
                        else if (skuExist == null)
                        {
                            await _redisRepository.AddProductUpdateMessage(new ProductUpdateMessage
                            {
                                Sku = item.Sku,
                                ProductVariantId = productVariant.Id
                            });
                        }
                        
                        
                      
                        
                        // if (productVariant.Sku== null && skuExist != null)
                      
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

        public async Task<List<string>> CheckSufficientReceiptQuantity(GoodsReceiptOrder goodsReceiptOrder)
        {
            var allRosOfPo = await _roRepository.GetAllGoodsReceiptsOfPurchaseOrder(goodsReceiptOrder.PurchaseOrderId);
            var po = await _poAsyncRepository.GetByIdAsync(goodsReceiptOrder.PurchaseOrderId);

            List<string> insufficientVariantIds = new List<string>();
            Dictionary<string, int> ProductVariantAndQuantityReceived = new Dictionary<string, int>();
            
            foreach (var receiptOrder in allRosOfPo)
            {
                foreach (var roi in receiptOrder.ReceivedOrderItems)
                {
                    if (ProductVariantAndQuantityReceived.ContainsKey(roi.ProductVariantId))
                    {
                        ProductVariantAndQuantityReceived.TryGetValue(roi.ProductVariantId, out var quantityDic);
                        var newQuantity = roi.QuantityReceived + quantityDic;
                        ProductVariantAndQuantityReceived.Add(roi.ProductVariantId, newQuantity);
                    }

                    else
                    {
                        var newQuantity = roi.QuantityReceived;
                        ProductVariantAndQuantityReceived.Add(roi.ProductVariantId, newQuantity);
                    }
                }
            }
            
            foreach (var orderItem in po.PurchaseOrderProduct)
            {
                if (ProductVariantAndQuantityReceived.ContainsKey(orderItem.ProductVariantId))
                {
                    if (ProductVariantAndQuantityReceived[orderItem.ProductVariantId] < orderItem.OrderQuantity)
                    {
                        if (!insufficientVariantIds.Contains(orderItem.ProductVariantId))
                            insufficientVariantIds.Add(orderItem.ProductVariantId);
                    }
                }
            }

            return insufficientVariantIds;
        }
        
        public async Task<List<ExistRedisVariantSKU>> CheckSkuExistance(PurchaseOrder po)
        {
            var redisData = await _redisRepository.GetProductUpdateMessage();
            List<ExistRedisVariantSKU> returnList = new List<ExistRedisVariantSKU>();
            foreach (var goodsReceiptOrderItem in po.PurchaseOrderProduct)
            {
                var productVariantSKU = goodsReceiptOrderItem.ProductVariant.Sku;
                var redisSKUForVariant =
                    redisData.FirstOrDefault(r => r.ProductVariantId == goodsReceiptOrderItem.ProductVariantId); 
                if((string.IsNullOrEmpty(productVariantSKU) && 
                   redisSKUForVariant !=null) || (!string.IsNullOrEmpty(productVariantSKU) && 
                                                  redisSKUForVariant !=null))
                    returnList.Add(new ExistRedisVariantSKU
                    {
                        ProductVariantId = goodsReceiptOrderItem.ProductVariantId,
                        RedisSKU = redisSKUForVariant.Sku
                    });
                
            }

            return returnList;
        }
    }
}