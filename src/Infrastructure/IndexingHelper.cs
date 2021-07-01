using System;
using System.Diagnostics;
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
                CreatedDate =  (po.Transaction.TransactionRecord.Count > 0) ? po.Transaction.TransactionRecord[0].Date : DateTime.MinValue,
                ModifiedDate = (po.Transaction.TransactionRecord.Count > 0) ? po.Transaction.TransactionRecord[^1].Date : DateTime.MinValue,
                DeliveryDate = po.DeliveryDate,
                TotalPrice = (po.TotalOrderAmount != null) ? po.TotalOrderAmount : 0,
                ConfirmedByName = (po.Transaction.ApplicationUser != null) ? po.Transaction.ApplicationUser.Fullname : "",
                SupplierEmail = (po.Supplier != null) ? po.Supplier.Email : "",
                SupplierId = (po.Supplier != null) ? po.Supplier.Id : "",
                SupplierPhone = (po.Supplier != null) ? po.Supplier.PhoneNumber : "",
                CanceledByName = (po.Transaction.ApplicationUser != null) ? po.Transaction.ApplicationUser.Fullname : "",
                CreatedByName = (po.Transaction.ApplicationUser != null) ? po.Transaction.ApplicationUser.Fullname : ""
            };
            
            index.FillSuggestion();
            return index;
        }

        public static GoodsReceiptOrderSearchIndex GoodsReceiptOrderSearchIndex(GoodsReceiptOrder ro)
        {
            Debug.Assert(ro.Transaction != null, "ro.Transaction != null");
            var index = new GoodsReceiptOrderSearchIndex
            {
                TransactionId = (ro.Transaction!=null) ? ro.Transaction.Id : "",
                Id = ro.Id,
                PurchaseOrderId = (ro.PurchaseOrderId!=null) ? ro.PurchaseOrderId : "",
                SupplierName = (ro.Supplier!=null) ? ro.Supplier.SupplierName : "",
                CreatedBy = (ro.Transaction.ApplicationUser!=null) ? ro.Transaction.ApplicationUser.Fullname : "" ,
                CreatedDate = (ro.Transaction.TransactionRecord.Count > 0) ? ro.Transaction.TransactionRecord[0].Date : DateTime.MinValue
            };

            return index;
        }
        
        public static GoodsIssueSearchIndex GoodsIssueSearchIndexHelper(GoodsIssueOrder gi)
        {
            Debug.Assert(gi.Transaction != null, "gi.Transaction != null");
            var index = new GoodsIssueSearchIndex
            {
                TransactionId = (gi.Transaction!=null) ? gi.Transaction.Id : "",
                Id = gi.Id,
                Status = gi.GoodsIssueType.ToString(),
                CreatedByName = (gi.Transaction.ApplicationUser!=null) ? gi.Transaction.ApplicationUser.Fullname : "" ,
                CreatedDate =  (gi.Transaction.TransactionRecord.Count > 0) ? gi.Transaction.TransactionRecord[0].Date : DateTime.MinValue,
                DeliveryDate = gi.DeliveryDate,
                DeliveryMethod = gi.DeliveryMethod,
                GoodsIssueNumber = gi.Id,
                GoodsIssueRequestNumber = gi.RequestId
            };

            return index;
        }
        
        public static ProductVariantSearchIndex ProductVariantSearchIndex(Product product)
        {
            ProductVariantSearchIndex index = null; 
            foreach (var productVariant in product.ProductVariants)
            {
                try
                {
                    Debug.Assert(productVariant.Transaction != null, "productVariant.Transaction != null");
                    index = new ProductVariantSearchIndex
                    {
                        TransactionId = (productVariant.Transaction!=null) ? productVariant.TransactionId : "" ,
                        Id = productVariant.Id,
                        Name = productVariant.Name,
                        ProductId = productVariant.ProductId,
                        ProductVariantId = productVariant.Id,
                        Category = (product.Category != null) ? product.Category.CategoryName : "",
                        Quantity = productVariant.StorageQuantity,
                        ModifiedDate = (product.Transaction.TransactionRecord.Count > 0) ? product.Transaction.TransactionRecord[^1].Date : DateTime.MinValue,
                        Sku = productVariant.Sku,
                        Brand = (product.Brand!=null) ? product.Brand.BrandName : "",
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
        
        public static ProductSearchIndex ProductSearchIndex(Product product)
        {
            ProductSearchIndex index = null; 
            try
            {
                Debug.Assert(product.Transaction != null, "product.Transaction != null");
                index = new ProductSearchIndex
                {
                    TransactionId = (product.Transaction!=null) ? product.TransactionId : "" ,
                    Id = product.Id,
                    Name = product.Name,
                    ProductId = product.Id,
                    Category = (product.Category != null) ? product.Category.CategoryName : "",
                    ModifiedDate = (product.Transaction.TransactionRecord.Count > 0) ? product.Transaction.TransactionRecord[^1].Date : DateTime.MinValue,
                    Brand = (product.Brand!=null) ? product.Brand.BrandName : "",
                    Strategy = product.SellingStrategy,
                    CreatedDate =(product.Transaction.TransactionRecord.Count > 0) ? product.Transaction.TransactionRecord[0].Date : DateTime.MinValue,
                    CreatedByName = (product.Transaction.ApplicationUser != null) ? product.Transaction.ApplicationUser.Fullname : "",
                    ModifiedByName =(product.Transaction.ApplicationUser != null) ? product.Transaction.ApplicationUser.Fullname : "",
                    IsVariantType = product.IsVariantType,
                };
                
                foreach (var productProductVariant in product.ProductVariants)
                    index.Variants.Add(ProductVariantSearchIndex(productProductVariant));
                
                index.FillSuggestion();
                
            }
            catch (Exception e)
            {
                Console.WriteLine(product.Id);
                Console.WriteLine(e);
                throw;
            }

            return index;
        }
        
        public static StockTakeSearchIndex StockTakeSearchIndex(StockTakeOrder stockTake)
        {
            StockTakeSearchIndex index = null;
            if (stockTake.Transaction != null)
                index = new StockTakeSearchIndex
                {
                    TransactionId = (stockTake.Transaction != null) ? stockTake.TransactionId : "",
                    Id = stockTake.Id,
                    Status = stockTake.StockTakeOrderType.ToString(),
                    CreatedDate = stockTake.Transaction.TransactionRecord[0].Date,
                    ModifiedDate = stockTake.Transaction.TransactionRecord[^1].Date,
                    CreatedByName = (stockTake.Transaction.ApplicationUser != null)
                        ? stockTake.Transaction.ApplicationUser.Fullname
                        : ""
                };

            return index;
        }
        
        public static ProductVariantSearchIndex ProductVariantSearchIndex(ProductVariant productVariant)
        {
            ProductVariantSearchIndex index = null; 
                try
                {
                    Debug.Assert(productVariant.Transaction != null, "productVariant.Transaction != null");
                    index = new ProductVariantSearchIndex
                    {
                        TransactionId = (productVariant.Transaction!=null) ? productVariant.TransactionId : "",
                        Id = productVariant.Id,
                        Name = productVariant.Name,
                        ProductId = productVariant.ProductId,
                        ProductVariantId = productVariant.Id,
                        Category = (productVariant.Product.Category.CategoryName != null) ? productVariant.Product.Category.CategoryName : "",
                        Quantity = productVariant.StorageQuantity,
                        // ModifiedDate = (productVariant.Transaction.TransactionRecord.Count > 0) ? productVariant.Transaction.TransactionRecord[^1].Date : DateTime.MinValue,
                        Sku = productVariant.Sku,
                        Unit = productVariant.Unit,
                        Brand = (productVariant.Product.Brand != null) ? productVariant.Product.Brand.BrandName : "",
                        Price = productVariant.Price,
                        Strategy = (productVariant.Product.SellingStrategy!= null) ? productVariant.Product.SellingStrategy : "",
                        // CreatedDate =  (productVariant.Transaction.TransactionRecord.Count > 0) ? productVariant.Transaction.TransactionRecord[0].Date : DateTime.MinValue,
                        CreatedByName = (productVariant.Transaction.ApplicationUser) != null ? productVariant.Transaction.ApplicationUser.Fullname : "",
                        ModifiedByName = (productVariant.Transaction.ApplicationUser) != null ? productVariant.Transaction.ApplicationUser.Fullname : "",
                        // SupplierName = productVariant.Packages[^1].Supplier.SupplierName
                    };

               
                    index.ModifiedDate = (productVariant.Transaction.TransactionRecord.Count > 0)
                        ? productVariant.Transaction.TransactionRecord[^1].Date
                        : DateTime.MinValue;
                    index.CreatedDate = (productVariant.Transaction.TransactionRecord.Count > 0)
                        ? productVariant.Transaction.TransactionRecord[0].Date
                        : DateTime.MinValue;

                    index.SupplierName = (productVariant.Packages.Count > 0)
                        ? productVariant.Packages[^1].Supplier.SupplierName
                        : "";
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

        public static BrandSearchIndex ProductVariantSearchIndex(Brand brand)
        {
            try
            {
                var brandSearchIndex = new BrandSearchIndex
                {
                    BrandId = brand.Id,
                    BrandName = brand.BrandName,
                };
                
                foreach (var brandProduct in brand.Products)
                {
                    brandSearchIndex.BrandProductIndexInfos.Add(new BrandProductIndexInfo
                    {
                        ProductId = brandProduct.Id,
                        ProductName = brandProduct.Name
                    });
                }

                return brandSearchIndex;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}