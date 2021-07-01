using System;
using System.Collections.Generic;
using System.IO;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.BigQuery.V2;
using InventoryManagementSystem.ApplicationCore.Entities.Products;

namespace Infrastructure.Services
{
    public class BigQueryService
    {
        private const string GCC_PROJECTID = "imswarehouse";
        private BigQueryClient _bigQueryClient;
        public BigQueryService()
        {
            
            _bigQueryClient = BigQueryClient.Create(GCC_PROJECTID,
                GoogleCredential.FromStream(new StreamReader(@"/home/thomasm/InventoryManagementSystem/src/Infrastructure/imswarehouse-70a47bcd9c79.json").BaseStream));
        }

        public void InsertProductRowToAPI(ProductVariant productVariant, int quantitySold)
        {
            
            var dataset = _bigQueryClient.ListDatasets(GCC_PROJECTID, new ListDatasetsOptions());
            BigQueryTable productTable = null;
            BigQueryTable factTable = null;

            string query = @"SELECT * FROM fact ORDER BY productsk DESC LIMIT 1";
            
            var result = _bigQueryClient.ExecuteQuery(query, parameters: null);
            ulong latestRecordNumber = 0;
            if (result.TotalRows != null)
                latestRecordNumber += result.TotalRows.Value;


            foreach (var bigQueryDataset in dataset)
            {
                foreach (var bigQueryTable in bigQueryDataset.ListTables())
                {
                    if (bigQueryTable.FullyQualifiedId.Contains("productdim"))
                        productTable = bigQueryTable;
                }
            }
            
            List<BigQueryInsertRow> listRowInser = new List<BigQueryInsertRow>();
            Dictionary<string, object> columnValues = new Dictionary<string, object>();
            BigQueryInsertRow row = new BigQueryInsertRow();
            
            
            foreach (var productVariantPackage in productVariant.Packages)
            {
                columnValues.Add("productsk", latestRecordNumber);
                columnValues.Add("productvariantid", productVariantPackage.ProductVariant.Id);
                columnValues.Add("productcategory", productVariantPackage.ProductVariant.Product.Category.CategoryName);
                columnValues.Add("price", Convert.ToSingle(productVariantPackage.Price));
                columnValues.Add("storagelocation", productVariantPackage.Location);
                columnValues.Add("date", DateTime.Now);
                columnValues.Add("cost", productVariantPackage.ProductVariant.Cost);
                columnValues.Add("quantityavailable", productVariantPackage.Quantity);
                columnValues.Add("quantitysold", quantitySold);
                columnValues.Add("transactiontype", productVariantPackage.ProductVariant.Transaction.Type);
            }
       
            
            row.Add(columnValues);
            listRowInser.Add(row);
            _bigQueryClient.InsertRows(productTable.Reference, listRowInser);
        }
    }
}