using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using InventoryManagementSystem.ApplicationCore.Entities;
using InventoryManagementSystem.ApplicationCore.Entities.Orders;
using InventoryManagementSystem.ApplicationCore.Entities.Orders.Status;
using InventoryManagementSystem.ApplicationCore.Entities.Products;
using InventoryManagementSystem.ApplicationCore.Entities.SearchIndex;

namespace InventoryManagementSystem.ApplicationCore.Services
{
    public class FilteringService
    {
             public static List<PurchaseOrderSearchIndex> PurchaseOrderIndexFiltering(List<PurchaseOrderSearchIndex> resource, POSearchFilter poSearchFilter, CancellationToken cancellationToken  = default)
             {
                 var pos = resource.Where(po =>
                         ( (poSearchFilter.Statuses == null ||
                            poSearchFilter.Statuses.Contains((EnumUtil.ParseEnum<PurchaseOrderStatusType>(po.Status).ToString()) ))
                           &&
                           (poSearchFilter.HideMerged == false ||
                            po.Status != PurchaseOrderStatusType.RequisitionMerged.ToString())      
                         &&
                         (poSearchFilter.FromDeliveryDate == null ||
                          (po.DeliveryDate >= DateTime.Parse(poSearchFilter.FromDeliveryDate) &&
                           po.DeliveryDate <= DateTime.Parse(poSearchFilter.ToDeliveryDate))) 
                         
                         &&
                         (poSearchFilter.FromCreatedDate == null ||
                          (po.CreatedDate >= DateTime.Parse(poSearchFilter.FromCreatedDate) &&
                           po.CreatedDate <= DateTime.Parse(poSearchFilter.ToCreatedDate))) 
                         
                         &&
                         (poSearchFilter.FromTotalOrderPrice == null ||
                          (po.TotalPrice >= Decimal.Parse(poSearchFilter.FromTotalOrderPrice) &&
                           po.TotalPrice <= Decimal.Parse(poSearchFilter.ToTotalOrderPrice))) 
                         
                         &&
                         (poSearchFilter.SupplierId == null || po.SupplierId == poSearchFilter.SupplierId) 
                         
                         &&
                         (poSearchFilter.CreatedByName == null || po.CreatedByName == poSearchFilter.CreatedByName)
                         &&
                         (poSearchFilter.FromModifiedDate == null ||
                          (po.ModifiedDate >= DateTime.Parse(poSearchFilter.FromModifiedDate) &&
                           po.ModifiedDate <= DateTime.Parse(poSearchFilter.ToModifiedDate)))
                         &&
                         (poSearchFilter.FromConfirmedDate == null ||
                          (po.ConfirmedDate >= DateTime.Parse(poSearchFilter.FromConfirmedDate) &&
                           po.ConfirmedDate <= DateTime.Parse(poSearchFilter.ToConfirmedDate)))
                         &&
                         (poSearchFilter.ConfirmedByName == null || po.ConfirmedByName == poSearchFilter.ConfirmedByName)
                         &&
                         (poSearchFilter.IgnoreOrderIds == null || !poSearchFilter.IgnoreOrderIds.Contains(po.Id)
                         
                     )))
                     .ToList();
                 return pos;
             }
             
                  public static List<GoodsReceiptOrderSearchIndex> ReceivingOrderIndexFiltering(List<GoodsReceiptOrderSearchIndex> resource, ROSearchFilter roSearchFilter, CancellationToken cancellationToken = default)
                     {
                         var ros = resource.Where(ro =>
                                 
                                 (roSearchFilter.CreatedByName == null || ro.CreatedBy == roSearchFilter.CreatedByName)
                                 &&
                                 (roSearchFilter.FromCreatedDate == null ||
                                  (ro.CreatedDate >= DateTime.Parse(roSearchFilter.FromCreatedDate) &&
                                   ro.CreatedDate <= DateTime.Parse(roSearchFilter.ToCreatedDate))) 
                                 &&
                                 (roSearchFilter.SupplierName == null || ro.SupplierName == roSearchFilter.SupplierName)
                             )
                             .ToList();
                         return ros;
                     }
             
                     public static List<GoodsIssueSearchIndex> GoodsIssueIndexFiltering(List<GoodsIssueSearchIndex> resource, GISearchFilter giSearchFilter, CancellationToken cancellationToken =default)
                     {
                         var ros = resource.Where(gi =>
                                 
                                 ((giSearchFilter.Statuses == null ||
                                  giSearchFilter.Statuses.Contains(gi.Status))
                                
                                 &&
                                 (giSearchFilter.FromCreatedDate == null ||
                                  (gi.CreatedDate >= DateTime.Parse(giSearchFilter.FromCreatedDate) &&
                                   gi.CreatedDate <= DateTime.Parse(giSearchFilter.ToCreatedDate))) 
                                 &&
                                 (giSearchFilter.CreatedByName == null || gi.CreatedByName == giSearchFilter.CreatedByName)
                                 &&
                                 (giSearchFilter.DeliveryMethod == null || gi.DeliveryMethod == giSearchFilter.DeliveryMethod)
                                 &&
                                 (giSearchFilter.FromDeliveryDate == null ||
                                  (gi.DeliveryDate >= DateTime.Parse(giSearchFilter.FromDeliveryDate) &&
                                   gi.DeliveryDate <= DateTime.Parse(giSearchFilter.ToDeliveryDate))) 
                             ))
                             .ToList();
                         return ros;
                     }
             
                     public  static List<ProductVariantSearchIndex> ProductVariantIndexFiltering(List<ProductVariantSearchIndex> resource, ProductVariantSearchFilter productSearchFilter, CancellationToken cancellationToken)
                     {
                         var pos = resource.Where(product =>
                             ( 
                                 (productSearchFilter.FromCreatedDate == null ||
                                  (product.CreatedDate >= DateTime.Parse(productSearchFilter.FromCreatedDate) &&
                                   product.CreatedDate <= DateTime.Parse(productSearchFilter.ToCreatedDate))) 
                                 
                                 &&
                                 (productSearchFilter.FromModifiedDate == null ||
                                  (product.ModifiedDate >= DateTime.Parse(productSearchFilter.FromModifiedDate) &&
                                   product.ModifiedDate <= DateTime.Parse(productSearchFilter.ToModifiedDate))) 
                                 
                                 &&
                                 (productSearchFilter.Category == null ||
                                  (product.Category == productSearchFilter.Category) )
                                 
                                  &&
                                  (productSearchFilter.Strategy == null ||
                                   (product.Strategy == productSearchFilter.Strategy) )
                                 
                                   &&
                                   (productSearchFilter.CreatedByName == null || product.CreatedByName == productSearchFilter.CreatedByName)
                                  
                                   &&
                                   (productSearchFilter.ModifiedByName == null || product.ModifiedByName == productSearchFilter.ModifiedByName)
                                  
                                  
                                   &&
                                   (productSearchFilter.FromPrice == null || 
                                    (product.Price >= Decimal.Parse(productSearchFilter.FromPrice)
                                     && product.Price <= Decimal.Parse(productSearchFilter.ToPrice)
                                    ) )
                                   &&
                                   (productSearchFilter.Brand == null || product.Brand == productSearchFilter.Brand)
                                  ))
                             .ToList();
                         return pos;
                     }
             
                     public static List<Package> PackageIndexFiltering(List<Package> resource, PackageSearchFilter packageSearchFilter,
                         CancellationToken cancellationToken)
                     {
                         var packages = resource.Where(package =>
                             ( 
                                 package.Location!=null 
                                 &&
                                 ((packageSearchFilter.FromImportedDate == null)  ||
                                  (package.ImportedDate >= DateTime.Parse(packageSearchFilter.FromImportedDate) &&
                                   package.ImportedDate <= DateTime.Parse(packageSearchFilter.ToImportedDate))) 
                                 
                                 &&
                                 (packageSearchFilter.FromPrice == null ||
                                  (package.Price >= Decimal.Parse(packageSearchFilter.FromPrice) &&
                                   package.Price <= Decimal.Parse(packageSearchFilter.ToPrice))) 
                                 
                                 &&
                                 (packageSearchFilter.FromTotalPrice == null ||
                                  (package.TotalPrice >= Decimal.Parse(packageSearchFilter.FromTotalPrice) &&
                                   package.TotalPrice <= Decimal.Parse(packageSearchFilter.ToTotalPrice))) 
                                 
                                 &&
                                 (packageSearchFilter.FromQuantity == null ||
                                  (package.Quantity >= int.Parse(packageSearchFilter.FromQuantity) &&
                                   package.Quantity <= int.Parse(packageSearchFilter.ToQuantity))) 
                                 
                                 &&
                                 (packageSearchFilter.LocationId == null  ||
                                  (package.Location.Id == packageSearchFilter.LocationId) )
                                 
                                  &&
                                  ((packageSearchFilter.ProductVariantID == null ) ||
                                   (package.ProductVariantId == packageSearchFilter.ProductVariantID))
                                 ))
                             .ToList();
                         return packages;
                     }
             
                     public  static List<ProductSearchIndex> ProductIndexFiltering(List<ProductSearchIndex> resource, ProductSearchFilter productSearchFilter, CancellationToken cancellationToken)
                     {
                         var pos = resource.Where(product =>
                             ( 
                                 (productSearchFilter.FromCreatedDate == null ||
                                  (product.CreatedDate >= DateTime.Parse(productSearchFilter.FromCreatedDate) &&
                                   product.CreatedDate <= DateTime.Parse(productSearchFilter.ToCreatedDate))) 
                                 &&
                                 (productSearchFilter.FromModifiedDate == null ||
                                  (product.ModifiedDate >= DateTime.Parse(productSearchFilter.FromModifiedDate) &&
                                   product.ModifiedDate <= DateTime.Parse(productSearchFilter.ToModifiedDate))) 
                                 &&
                                 (productSearchFilter.Category == null ||
                                  (product.Category == productSearchFilter.Category))
                                 
                                  &&
                                  (productSearchFilter.Strategy == null ||
                                   (product.Strategy == productSearchFilter.Strategy))
                                 
                                   &&
                                   (productSearchFilter.CreatedByName == null || product.CreatedByName == productSearchFilter.CreatedByName)
                                  
                                   &&
                                   (productSearchFilter.ModifiedByName == null || product.ModifiedByName == productSearchFilter.ModifiedByName)
                                   &&
                                   (productSearchFilter.Brand == null || product.Brand == productSearchFilter.Brand)
                                  ))
                             .ToList();
                         return pos;
                     }
                     
             
                     public static List<StockTakeSearchIndex> StockTakeIndexFiltering(List<StockTakeSearchIndex> resource, STSearchFilter stSearchFilter, CancellationToken cancellationToken = default)
                     {
                         var sts = resource.Where(st =>
                             ( 
                                 (stSearchFilter.Statuses == null ||
                                  stSearchFilter.Statuses.Contains((EnumUtil.ParseEnum<StockTakeOrderType>(st.Status).ToString()))
                                 
                                  &&
                                  (stSearchFilter.FromCreatedDate == null ||
                                   (st.CreatedDate >= DateTime.Parse(stSearchFilter.FromCreatedDate) &&
                                    st.CreatedDate <= DateTime.Parse(stSearchFilter.ToCreatedDate))) 
                                 
                                  &&
                                  (stSearchFilter.FromDeliveryDate == null ||
                                   (st.ModifiedDate >= DateTime.Parse(stSearchFilter.FromDeliveryDate) &&
                                    st.ModifiedDate <= DateTime.Parse(stSearchFilter.ToDeliveryDate))) 
                                 
                            
                                  &&
                                  (stSearchFilter.CreatedByName == null || st.CreatedByName == stSearchFilter.CreatedByName)
                                 )))
                             .ToList();
                         return sts;
                     }

                     public static List<UserAndRole> UserAndRolesIndexFiltering(List<UserAndRole> resources,
                         UserInfoFilter userInfoFilter, CancellationToken cancellationToken = default)
                     {
                         var userFilter = resources.Where(u => (
                             ((userInfoFilter.SearchQuery == null ||
                             u.ImsUser.Fullname.Contains(userInfoFilter.SearchQuery)
                            )
                             
                             ||
                             (userInfoFilter.SearchQuery == null ||
                              u.ImsUser.Email.Contains(userInfoFilter.SearchQuery) 
                             ))
                             
                             &&
                             (userInfoFilter.Role == null ||
                              userInfoFilter.Role.Contains(u.UserRole)
                             )

                             &&
                             (userInfoFilter.Status == null ||
                                 userInfoFilter.Status.Contains(u.ImsUser.IsActive))

                         )).ToList();

                         return userFilter;
                     }
    }
}