using System;
using Elasticsearch.Net;
using InventoryManagementSystem.ApplicationCore.Entities.Orders;
using InventoryManagementSystem.ApplicationCore.Entities.Orders.Status;
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
                TransactionId = (po.Transaction!=null) ? po.Transaction.Id : "",
                Id = po.Id,
                SupplierName = (po.Supplier != null) ? po.Supplier.SupplierName : "",
                Status = (po.PurchaseOrderStatus.GetStringValue() != null)
                    ? po.PurchaseOrderStatus.GetStringValue()
                    : "",
                CreatedDate = po.Transaction.CreatedDate,
                ModifiedDate = po.Transaction.ModifiedDate,
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


            return index;
        }

        public static GoodsReceiptOrderSearchIndex GoodsReceiptOrderSearchIndex(GoodsReceiptOrder ro)
        {
            var index = new GoodsReceiptOrderSearchIndex
            {
                TransactionId = (ro.Transaction!=null) ? ro.Transaction.Id : "",
                Id = ro.Id,
                PurchaseOrderId = (ro.PurchaseOrderId!=null) ? ro.PurchaseOrderId : "",
                SupplierName = (ro.Supplier!=null) ? ro.Supplier.SupplierName : "",
                CreatedBy = (ro.Transaction.CreatedBy!=null) ? ro.Transaction.CreatedBy.Fullname : "" ,
                CreatedDate = ro.Transaction.CreatedDate
            };

            return index;
        }
        
        public static GoodsIssueSearchIndex GoodsIssueSearchIndexHelper(GoodsIssueOrder gi)
        {
            var index = new GoodsIssueSearchIndex
            {
                TransactionId = (gi.Transaction!=null) ? gi.Transaction.Id : "",
                Id = gi.Id,
                Status = gi.GoodsIssueType.ToString(),
                CreatedByName = (gi.Transaction.CreatedBy!=null) ? gi.Transaction.CreatedBy.Fullname : "" ,
                CreatedDate =  gi.Transaction.CreatedDate,
                DeliveryDate = gi.DeliveryDate,
                DeliveryMethod = gi.DeliveryMethod,
                GoodsIssueNumber = gi.Id,
                GoodsIssueRequestNumber = gi.RequestId
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
                        TransactionId = (productVariant.Transaction!=null) ? productVariant.TransactionId : "" ,
                        Id = productVariant.Id,
                        Name = nameConcat,
                        ProductId = productVariant.ProductId,
                        VariantId = productVariant.Id,
                        Category = (product.Category != null) ? product.Category.CategoryName : "",
                        Quantity = productVariant.StorageQuantity,
                        ModifiedDate = productVariant.Transaction.ModifiedDate,
                        Sku = productVariant.Sku,
                        Brand = product.BrandName,
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
        
        public static StockTakeSearchIndex StockTakeSearchIndex(StockTakeOrder stockTake)
        {
            StockTakeSearchIndex index = null;
            index = new StockTakeSearchIndex
            {
                TransactionId = (stockTake.Transaction!=null) ? stockTake.TransactionId : "",
                Id = stockTake.Id,
                Status = stockTake.StockTakeOrderType.ToString(),
                CreatedDate = stockTake.Transaction.CreatedDate,
                ModifiedDate = stockTake.Transaction.ModifiedDate,
                CreatedByName = (stockTake.Transaction.CreatedBy != null) ? stockTake.Transaction.CreatedBy.Fullname : ""
            };

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
                        TransactionId = (productVariant.Transaction!=null) ? productVariant.TransactionId : "",
                        Id = productVariant.Id,
                        Name = nameConcat,
                        ProductId = productVariant.ProductId,
                        VariantId = productVariant.Id,
                        Category = (productVariant.Product.Category.CategoryName != null) ? productVariant.Product.Category.CategoryName : "",
                        Quantity = productVariant.StorageQuantity,
                        ModifiedDate = productVariant.Transaction.ModifiedDate,
                        Sku = productVariant.Sku,
                        Unit = productVariant.Unit,
                        Brand = (productVariant.Product.BrandName != null) ? productVariant.Product.BrandName : "",
                        Price = productVariant.Price,
                        Strategy = (productVariant.Product.SellingStrategy!= null) ? productVariant.Product.SellingStrategy : "",
                        CreatedDate = productVariant.Transaction.CreatedDate,
                        CreatedByName = productVariant.Transaction.CreatedBy.Fullname,
                        ModifiedByName = productVariant.Transaction.ModifiedBy.Fullname,
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