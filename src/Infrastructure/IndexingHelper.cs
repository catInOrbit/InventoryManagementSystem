using System;
using Elasticsearch.Net;
using InventoryManagementSystem.ApplicationCore.Entities.Orders;
using InventoryManagementSystem.ApplicationCore.Entities.Products;
using InventoryManagementSystem.ApplicationCore.Entities.SearchIndex;

namespace Infrastructure
{
    public class IndexingHelper
    {
        public static PurchaseOrderSearchIndex PurchaseOrderSearchIndex(PurchaseOrder po)
        {
            var index = new PurchaseOrderSearchIndex
            {
                Id = po.Id,
                SupplierName = (po.Supplier != null) ? po.Supplier.SupplierName : "",
                Status = (po.PurchaseOrderStatus.GetStringValue() != null)
                    ? po.PurchaseOrderStatus.GetStringValue()
                    : "",
                CreatedDate = po.Transaction.CreatedDate,
                DeliveryDate = po.DeliveryDate,
                TotalPrice = (po.TotalOrderAmount != null) ? po.TotalOrderAmount : 0,
                ConfirmedByName = (po.Transaction.CreatedBy != null) ? po.Transaction.CreatedBy.Fullname : "",
                SupplierEmail = (po.Supplier != null) ? po.Supplier.Email : "",
                SupplierId = (po.Supplier != null) ? po.Supplier.Id : "",
                SupplierPhone = (po.Supplier != null) ? po.Supplier.PhoneNumber : "",
                CanceledByName = (po.Transaction.CreatedBy != null) ? po.Transaction.CreatedBy.Fullname : "",
                CreatedByName = (po.Transaction.CreatedBy != null) ? po.Transaction.CreatedBy.Fullname : ""
            };
            
            index.FillSuggestion();

            //
            // var index = new PurchaseOrderSearchIndex();
            // index.Id = po.Id;
            // index.SupplierName = (po.Supplier != null) ? po.Supplier.SupplierName : "";
            // index.PurchaseOrderNumber = (po.PurchaseOrderNumber != null) ? po.PurchaseOrderNumber : "";
            // index.Status = (po.PurchaseOrderStatus.GetStringValue() != null)
            //     ? po.PurchaseOrderStatus.GetStringValue()
            //     : "";
            // index.CreatedDate = po.Transaction.CreatedDate;
            // index.DeliveryDate = po.DeliveryDate;
            // index.TotalPrice = (po.TotalOrderAmount != null) ? po.TotalOrderAmount : 0;
            // index.ConfirmedByName = (po.Transaction.CreatedBy != null) ? po.Transaction.CreatedBy.Fullname : "";
            // index.SupplierEmail = (po.Supplier != null) ? po.Supplier.Email : "";
            // index.SupplierId = (po.Supplier != null) ? po.Supplier.Id : "";
            // index.SupplierPhone = (po.Supplier != null) ? po.Supplier.PhoneNumber : "";
            // index.CanceledByName = (po.Transaction.CreatedBy != null) ? po.Transaction.CreatedBy.Fullname : "";
            // index.CreatedByName = (po.Transaction.CreatedBy != null) ? po.Transaction.CreatedBy.Fullname : "";

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
                CreatedDate = ro.Transaction.CreatedDate.ToShortDateString(),
                
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
                        Catagory = (product.Category != null) ? product.Category.CategoryName : "",
                        Quantity = productVariant.StorageQuantity,
                        ModifiedDate = productVariant.Transaction.ModifiedDate,
                        Sku = productVariant.Sku,
                    };
                    index.FillSuggestion();
                    
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
        
        public static ProductSearchIndex ProductSearchIndex(ProductVariant productVariant)
        {
            ProductSearchIndex index = null; 
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
                        Catagory = productVariant.Product.Category.CategoryName,
                        Quantity = productVariant.StorageQuantity,
                        ModifiedDate = productVariant.Transaction.ModifiedDate,
                        Sku = productVariant.Sku,
                    };
                    index.FillSuggestion();
                }
                catch (Exception e)
                {
                    Console.WriteLine(productVariant.Id);
                    Console.WriteLine(e);
                    throw;
                }
            return index;
        }

    }
}