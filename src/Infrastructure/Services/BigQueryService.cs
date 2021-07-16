using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Util.Store;
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
            var path = Path.Combine(Directory.GetCurrentDirectory(), @"imswarehouse-70a47bcd9c79.json");
            
            _bigQueryClient = BigQueryClient.Create(GCC_PROJECTID,
                GoogleCredential.FromStream(new StreamReader(path).BaseStream));
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
        public async Task<string> TrainMLWithLargestSoldProduct()
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
                                 where EXTRACT(DATE from date) <= CURRENT_DATETIME() AND name=@name
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

            string query_forecast = @"SELECT name, date AS timestamp, quantitysold,
                        NULL AS forecast_value,
                        NULL AS prediction_interval_lower_bound, 
                        NULL AS prediction_interval_upper_bound 

                        FROM ( SELECT name, EXTRACT(DATE from date) AS date, 
                        quantitysold FROM `imswarehouse.IMSWH01.mock10ksequential`
                        WHERE name=@name
                        GROUP BY name, date, quantitysold) 
                        UNION ALL 

                        SELECT name, EXTRACT(DATE from forecast_timestamp) AS timestamp, NULL AS historical_sale, forecast_value, prediction_interval_lower_bound, prediction_interval_upper_bound 

                        FROM ML.FORECAST(MODEL ML_Product.product_ml, STRUCT(100 AS horizon, 0.9 AS confidence_level))";
            
            job = _bigQueryClient.CreateQueryJob(
                sql: trainMLQuery,
                parameters: parameters,
                options: new QueryOptions
                {
                    UseQueryCache = false,
                });
            
            job = job.PollUntilCompleted().ThrowOnAnyError();
            BigQueryResults queryResult = _bigQueryClient.GetQueryResults(job.Reference.JobId,
                new GetQueryResultsOptions());

            StringBuilder stringBuilder = new StringBuilder();
            
            int count = 0;
            for (int i = 0; i <= queryResult.Schema.Fields.Count - 1; i++)
            {
                string columnname = "";
                var header = queryResult.Schema.Fields[0].Name;
                if (i + 1 >= queryResult.Schema.Fields.Count)
                    columnname = queryResult.Schema.Fields[i].Name;
                else
                    columnname = queryResult.Schema.Fields[i].Name + ",";
                stringBuilder.Append(columnname);
            }
            stringBuilder.Append(Environment.NewLine);
            await foreach (var row in queryResult.GetRowsAsync())
            {
                count++;
                if (count % 1000 == 0)
                    Console.WriteLine($"item {count} finished");
                int blub = queryResult.Schema.Fields.Count;
                for (Int64 j = 0; j < Convert.ToInt64(blub); j++)
                {
                    try
                    {
                        if (row.RawRow.F[Convert.ToInt32(j)] != null)
                            stringBuilder.Append(row.RawRow.F[Convert.ToInt32(j)].V + ",");

                    }
                    catch (Exception e)
                    {
                        throw new Exception(e.Message);
                    }
                }
                stringBuilder.Append(Environment.NewLine);
            }

            string csvoutputPath =Path.Combine(Directory.GetCurrentDirectory(), @"forecast_single_product.csv");
            File.WriteAllText(csvoutputPath, stringBuilder.ToString());
            
            return productName;
        }
    }

    public class GoogleSheetService
    {
        static string[] Scopes = { SheetsService.Scope.SpreadsheetsReadonly };
        static string ApplicationName = "Datasheet";

        public GoogleSheetService()
        {
            
            ServiceAccountCredential serviceAccountCredential;
            var jsonPath = Path.Combine(Directory.GetCurrentDirectory(), @"imswarehouse-70a47bcd9c79.json");
            string[] scopes = { SheetsService.Scope.Spreadsheets };
            string serviceAccountEmail = "my-bigquery-sa@imswarehouse.iam.gserviceaccount.com";
            using (Stream stream = new FileStream(jsonPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                serviceAccountCredential = (ServiceAccountCredential)
                    GoogleCredential.FromStream(stream).UnderlyingCredential;

                var initializer = new ServiceAccountCredential.Initializer(serviceAccountCredential.Id)
                {
                    User = serviceAccountEmail,
                    Key = serviceAccountCredential.Key,
                    Scopes = Scopes
                };
                serviceAccountCredential = new ServiceAccountCredential(initializer);
            }
            
            var service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = serviceAccountCredential,
                ApplicationName = ApplicationName,
            });

            string spreadSheetId ="1bev02YIKBl4YXxA-8eZety9fAalOseWRGzjhrJZ2Eqc";
            string range = "datasheet!A1:C";
       


            List<Object> row1 = new List<object>();
            row1.Add("Name");
            row1.Add("Rollno");
            row1.Add("Class");
            // similarly create more rows with data

            List<object> values = new List<object>();
            values.Add(row1);
            
            ValueRange valueRange = new ValueRange();
            valueRange.MajorDimension = "ROWS";
            valueRange.Values.Add(values);

            SpreadsheetsResource.ValuesResource.AppendRequest request =
                service.Spreadsheets.Values.Append(valueRange, spreadSheetId, range);
            request.InsertDataOption = SpreadsheetsResource.ValuesResource.AppendRequest.InsertDataOptionEnum.INSERTROWS;
            request.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;
            
            var response = request.Execute();
        }
    }
}