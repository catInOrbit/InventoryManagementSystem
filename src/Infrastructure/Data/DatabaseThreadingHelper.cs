using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InventoryManagementSystem.ApplicationCore.Entities;
using InventoryManagementSystem.ApplicationCore.Entities.Products;
using InventoryManagementSystem.ApplicationCore.Entities.SearchIndex;

namespace Infrastructure.Data
{
    public class DatabaseThreadingHelper
    {
        public PagingOption<ProductVariantSearchIndex> GetProductVariantForELIndexAsyncResourceOnly(List<ProductVariant> resources,PagingOption<ProductVariantSearchIndex> pagingOption)
        {
            // var variants =  resources.AsParallel().AsOrdered().
            //     Where(variant => variant.Transaction.TransactionRecord.Count > 0 && variant.Transaction.TransactionStatus!=false && variant.Transaction.Type!=TransactionType.Deleted).ToList();
            //
            // variants = variants.AsParallel().AsOrdered().OrderByDescending(e =>
            //     e.Transaction.TransactionRecord[e.Transaction.TransactionRecord.Count - 1].Date).ToList();
            //
            var resultList = new ConcurrentBag<ProductVariantSearchIndex>();
            
            
            Parallel.ForEach(resources, pr =>
            {
                try
                {
                    var index = IndexingHelper.ProductVariantSearchIndex(pr);
                    index.FillSuggestion();
                    resultList.Add(index);
                }
                catch (Exception e)
                {
                    Console.WriteLine(pr.Id);
                    Console.WriteLine(e);
                    throw;
                }
            });

            pagingOption.ResultList = resultList.ToList();
            pagingOption.ExecuteResourcePaging();

            return pagingOption;
        }
    }
}