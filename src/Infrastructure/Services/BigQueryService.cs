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
        private BigQueryClient bigQueryClient;
        public BigQueryService()
        {
            BigQueryClient bigQueryClient = BigQueryClient.Create(GCC_PROJECTID,
                GoogleCredential.FromStream(new StreamReader(@"/home/thomasm/RiderProjects/ConsoleApplication1/ConsoleApplication1/imswarehouse-0e17ae0451b2.json").BaseStream));
        }

        private static void InsertProductRowToAPI(BigQueryClient bigQueryClient, BigQueryTable productTable, ProductVariant productVariant)
        {
            List<BigQueryInsertRow> listRowInser = new List<BigQueryInsertRow>();

            Dictionary<string, object> columnValues = new Dictionary<string, object>();
    
            BigQueryInsertRow row = new BigQueryInsertRow();
            // columnValues.Add();
        }
    }
}