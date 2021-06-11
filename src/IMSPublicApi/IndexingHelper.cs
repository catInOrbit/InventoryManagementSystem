using System;
using Elasticsearch.Net;
using InventoryManagementSystem.ApplicationCore.Entities.Orders;
using InventoryManagementSystem.ApplicationCore.Entities.Products;
using InventoryManagementSystem.ApplicationCore.Entities.SearchIndex;

namespace InventoryManagementSystem.PublicApi
{
    public class IndexingHelper
    {
        public static PurchaseOrderSearchIndex PurchaseOrderSearchIndex(PurchaseOrder po)
        {
            var index = new PurchaseOrderSearchIndex
            {
                Id = po.Id,
                SupplierName = (po.Supplier != null) ? po.Supplier.SupplierName : "",
                PurchaseOrderNumber = (po.PurchaseOrderNumber != null) ? po.PurchaseOrderNumber : "",
                Status = (po.PurchaseOrderStatus.GetStringValue() != null)
                    ? po.PurchaseOrderStatus.GetStringValue()
                    : "",
                CreatedDate = po.Transaction.CreatedDate,
                DeliveryDate = po.DeliveryDate,
                TotalPrice = (po.TotalOrderAmount != null) ? po.TotalOrderAmount : 0,
                ConfirmedByName = (po.Transaction.CreatedBy != null) ? po.Transaction.CreatedBy.Fullname : ""
            };

            return index;
        }

        public static GoodsReceiptOrderSearchIndex GoodsReceiptOrderSearchIndex(GoodsReceiptOrder ro)
        {
            var index = new GoodsReceiptOrderSearchIndex
            {
                Id = ro.Id,
                PurchaseOrderId = (ro.PurchaseOrderId!=null) ? ro.PurchaseOrderId : "",
                SupplierName = (ro.Supplier!=null) ? ro.Supplier.SupplierName : "",
                CreatedBy = (ro.Transaction.CreatedBy!=null) ? ro.Transaction.CreatedBy.Fullname : "" ,
                ReceiptNumber = (ro.GoodsReceiptOrderNumber !=null) ? ro.GoodsReceiptOrderNumber : ""  ,
                CreatedDate = ro.Transaction.CreatedDate.ToShortDateString()
            };

            return index;
        }

        public static ProductSearchIndex ProductSearchIndex(Product product)
        {
            ProductSearchIndex index = null; 
            foreach (var productVariant in product.ProductVariants)
            {
                try
                {
                    string nameConcat = productVariant.Name;
                    foreach (var productVariantVariantValue in productVariant.VariantValues)
                    {
                        nameConcat += "-" + productVariantVariantValue.Value.Trim();
                    }
                
                    index = new ProductSearchIndex
                    {
                        Id = productVariant.Id,
                        Name = nameConcat,
                        ProductId = productVariant.ProductId,
                        VariantId = productVariant.Id,
                        Catagory = product.Category.CategoryName,
                        Quantity = productVariant.StorageQuantity,
                        ModifiedDate = productVariant.ModifiedDate
                    };
                }
                catch (Exception e)
                {
                    Console.WriteLine(productVariant.Id);
                    Console.WriteLine(e);
                    throw;
                }
            }

            return index;
        }
    }
}