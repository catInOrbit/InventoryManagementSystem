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

        public void InsertProductRowBQ(ProductVariant productVariant, decimal buy_price, 
            string storageLocation, int quantityAvailable,
            int quantitySold, decimal salePrice, string transactionType, string supplier)
        {
            
            var dataset = _bigQueryClient.ListDatasets(GCC_PROJECTID, new ListDatasetsOptions());
            BigQueryTable productTable = null;
            BigQueryTable factTable = null;

            
            foreach (var bigQueryDataset in dataset)
            {
                foreach (var bigQueryTable in bigQueryDataset.ListTables())
                {
                    if (bigQueryTable.FullyQualifiedId.Contains("productdim"))
                        productTable = bigQueryTable;
                }
            }
            
            BigQueryInsertRow row = new BigQueryInsertRow();
        
            row = new BigQueryInsertRow
            {
                {"productvariantid", productVariant.Id},
                {"name", productVariant.Name},
                {"category", productVariant.Product.Category.CategoryName},
                {"supplier", supplier},
                {"buy_price", Convert.ToSingle(buy_price)},
                {"storagelocation", storageLocation},
                {"sale_price", Convert.ToSingle(salePrice)},
                {"quantityavailable", quantityAvailable},
                {"quantitysold", quantitySold},
                {"transactiontype", transactionType},
                {"date", DateTime.UtcNow.ToUniversalTime().ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'")},
            };
            // columnValues.Add("productvariantid", productVariant.Id);
            // columnValues.Add("name", productVariant.Name);
            // columnValues.Add("category", productVariant.Product.Category.CategoryName);
            // columnValues.Add("buy_price", Convert.ToSingle(productVariantPackage.Price));
            // columnValues.Add("storagelocation", productVariantPackage.Location.LocationName);
            // columnValues.Add("sale_price", Convert.ToSingle(salePrice));
            // columnValues.Add("quantityavaialble", productVariantPackage.Quantity);
            // columnValues.Add("quantitysold", quantitySold);
            // columnValues.Add("transactiontype", transactionType);
            // columnValues.Add("date", DateTime.Now.ToUniversalTime().ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"));
            
            // row.Add(new Dictionary<string, object>(columnValues));
            // columnValues.Clear();
            productTable.InsertRow(row);
            // _bigQueryClient.InsertRow(productTable.Reference, row);
            
            // listRowInser.Add(row);
            // _bigQueryClient.InsertRows(productTable.Reference, listRowInser);
        }

        public BigQueryResults Get3LinesData()
        {
            var dataset = _bigQueryClient.ListDatasets(GCC_PROJECTID, new ListDatasetsOptions());
            BigQueryTable productTable = null;
            BigQueryTable factTable = null;
            
            string query = @"SELECT productname,date,quantitysold,cost, transactiontype, quantitysold*cost as totalvaluesold, quantityavailable * price as onhandvalue
                            FROM `imswarehouse.IMSWH01.mock2`
                            GROUP BY productname, date, quantitysold, cost,transactiontype, onhandvalue
                            ORDER BY date DESC";
            
            var result = _bigQueryClient.ExecuteQuery(query, parameters: null);
            
            return result;
        }
        
        
        /// <summary>
        /// Query product name with largest sold quantity and train machine learning model in bigqery
        /// </summary>
        /// <returns>Product name with largest sold quantity</returns>
        public string TrainMLWithLargestSoldProduct()
        {
            string queryLargestSoldProduct =
                @"Select a.name, a.quantitysold from `imswarehouse.IMSWH01.mock10ksequential` as a
                    inner join(
                        Select name, Max(quantitysold) as quantitysold 
                        from `imswarehouse.IMSWH01.mock10ksequential`
                        Group by name 
                    ) as b ON a.name = b.name and a.quantitysold = b.quantitysold 
                    ORDER BY a.quantitysold
                    DESC LIMIT 1";
            
            var result = _bigQueryClient.ExecuteQuery(queryLargestSoldProduct, parameters: null);
            string productName = null;
            
            foreach (var row in result)
                productName = row["name"].ToString();

            string trainMLQuery = @"create or replace model imswarehouse.ML_Product.product_ml
                                options
                                (
                                    model_type= 'ARIMA',
                                    time_series_timestamp_col = 'date',
                                    time_series_data_col = 'quantitysold',
                                    time_series_id_col = 'name'
                                 ) as
                                select name,
                                extract(date from date) as date,
                                 quantitysold
                                 from `imswarehouse.IMSWH01.mock10ksequential`
                                 where EXTRACT(DATE from date) <= CURRENT_DATETIME() AND name=?
                                 group by name,date,quantitysold";

            var parameters = new BigQueryParameter[]
            {
                new BigQueryParameter("name", BigQueryDbType.String, productName),
            };
            
            var job = _bigQueryClient.CreateQueryJob(
                sql: trainMLQuery,
                parameters: parameters,
                options: new QueryOptions
                {
                    UseQueryCache = false,
                });
            // Wait for the job to complete.
            job = job.PollUntilCompleted().ThrowOnAnyError();
            
            return productName;
        }
    }
}