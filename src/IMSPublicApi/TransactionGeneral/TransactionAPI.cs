using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Infrastructure;
using Infrastructure.Identity.DbContexts;
using Infrastructure.Services;
using InventoryManagementSystem.ApplicationCore.Constants;
using InventoryManagementSystem.ApplicationCore.Entities;
using InventoryManagementSystem.ApplicationCore.Entities.SearchIndex;
using InventoryManagementSystem.ApplicationCore.Interfaces;
using InventoryManagementSystem.ApplicationCore.Services;
using InventoryManagementSystem.PublicApi.AuthorizationEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Swashbuckle.AspNetCore.Annotations;

namespace InventoryManagementSystem.PublicApi.TransactionGeneral
{
    public class DeactivateTransaction : BaseAsyncEndpoint.WithRequest<DeleteTransactionRequest>.WithResponse<DeleteTransactionResponse>
    {
        private readonly IElasticAsyncRepository<ProductVariantSearchIndex> _productVariantAsyncRepository;
        private readonly IElasticAsyncRepository<ProductSearchIndex> _productAsyncRepository;

        private  readonly IConfiguration _configuration;

        private readonly IAuthorizationService _authorizationService;

        public DeactivateTransaction( IConfiguration configuration, IAuthorizationService authorizationService, IElasticAsyncRepository<ProductVariantSearchIndex> productVariantAsyncRepository, IElasticAsyncRepository<ProductSearchIndex> productAsyncRepository1)
        {
            _configuration = configuration;
            _authorizationService = authorizationService;
            _productVariantAsyncRepository = productVariantAsyncRepository;
            _productAsyncRepository = productAsyncRepository1;
        }


        [HttpPut("api/transactionstatus/{Id}")]
        [SwaggerOperation(
            Summary = "Change activation status of a transaction (Change status to false if true, vice versa)",
            Description = "Change activation status of a transaction (Change status to false if true, vice versa)",
            OperationId = "transac.changestatus",
            Tags = new[] { "TransactionEndpoints" })
        ]
        public override async Task<ActionResult<DeleteTransactionResponse>> HandleAsync([FromRoute] DeleteTransactionRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, PageConstant.TRANSACTION, UserOperations.Delete))
                return Unauthorized();
            var response = new DeleteTransactionResponse();

               DbContextOptionsBuilder<IdentityAndProductDbContext> options =
                new DbContextOptionsBuilder<IdentityAndProductDbContext>(
                );
            options.UseSqlServer(_configuration.GetConnectionString(ConnectionPropertiesConstant.MAIN_CONNECTION_STRING));
            await using (var dbContext = new IdentityAndProductDbContext(options.Options))
            {
                try
                {
                    var trans =  dbContext.Transaction.FirstOrDefault(e => e.Id == request.Id);
                    if ((int)trans.CurrentType == 7)
                    {
                        var productVariants = await dbContext.ProductVariant.Where(e => e.TransactionId == trans.Id).ToListAsync(cancellationToken: cancellationToken);
                        foreach (var productVariant in productVariants)
                        {
                            if (trans.TransactionStatus)
                            {
                                productVariant.Transaction.TransactionStatus = false;
                                await _productVariantAsyncRepository.ElasticDeleteSingleAsync(
                                    IndexingHelper.ProductVariantSearchIndex(productVariant), ElasticIndexConstant.PRODUCT_VARIANT_INDICES);    
                            }

                            else
                            {
                                productVariant.Transaction.TransactionStatus = true;
                                await _productVariantAsyncRepository.ElasticSaveSingleAsync(true,
                                    IndexingHelper.ProductVariantSearchIndex(productVariant), ElasticIndexConstant.PRODUCT_VARIANT_INDICES);    
                            }
                        }
                    }
                    
                    else if ((int)trans.CurrentType == 6)
                    {
                        var product = await dbContext.Product.FirstOrDefaultAsync(e => e.Transaction.Id == trans.Id);
                        if (trans.TransactionStatus)
                        {
                            product.Transaction.TransactionStatus = false;
                            await _productAsyncRepository.ElasticDeleteSingleAsync(
                                IndexingHelper.ProductSearchIndex(product), ElasticIndexConstant.PRODUCT_INDICES);   
                        }
                        else
                        {
                            await _productAsyncRepository.ElasticSaveSingleAsync(true,
                                IndexingHelper.ProductSearchIndex(product), ElasticIndexConstant.PRODUCT_INDICES);
                            product.Transaction.TransactionStatus = true;
                        }
                    }

                    else if ((int)trans.CurrentType == 9)
                    {
                        var category = await dbContext.Category.FirstOrDefaultAsync(e => e.Transaction.Id == trans.Id);
                        if (trans.TransactionStatus)
                            category.Transaction.TransactionStatus = false;
                        else
                            category.Transaction.TransactionStatus = true;
                    }
                    
                    else if ((int)trans.CurrentType == 8)
                    {
                        var supplier = await dbContext.Supplier.FirstOrDefaultAsync(e => e.Transaction.Id == trans.Id);
                        if (trans.TransactionStatus)
                            supplier.Transaction.TransactionStatus = false;
                        else
                            supplier.Transaction.TransactionStatus = true;
                    }

                    else
                    {
                        response.Status = false;
                        response.Verbose = "Deactivation of this transaction is not supported or transaction not found";
                        return Ok(response);
                    }
                    
                    response.Status = true;
                    response.Verbose = "Status changed";
                    await dbContext.SaveChangesAsync();
                    return Ok(response);
                }
                catch
                {
                    response.Status = false;
                    response.Verbose = "Id of transaction not found or internal error";
                    return NotFound(response);
                }
              
            }
        }
    }
    
    // public class DeleteTransaction : BaseAsyncEndpoint.WithRequest<DeleteTransactionRequest>.WithResponse<DeleteTransactionResponse>
    // {
    //     private IAsyncRepository<Transaction> _transactionRepository;
    //     private  readonly IConfiguration _configuration;
    //
    //     public DeleteTransaction(IAsyncRepository<Transaction> transactionRepository, IConfiguration configuration)
    //     {
    //         _transactionRepository = transactionRepository;
    //         _configuration = configuration;
    //     }
    //
    //     [HttpDelete("api/transaction/{Id}")]
    //     [SwaggerOperation(
    //         Summary = "Delete a transaction (Remove completely from database)",
    //         Description = "Delete a transaction (Remove completely from database)",
    //         OperationId = "transac.delete",
    //         Tags = new[] { "TransactionEndpoints" })
    //     ]
    //
    //     public override async Task<ActionResult<DeleteTransactionResponse>> HandleAsync([FromRoute]DeleteTransactionRequest request, CancellationToken cancellationToken = new CancellationToken())
    //     {
    //         var response = new DeleteTransactionResponse();
    //          DbContextOptionsBuilder<IdentityAndProductDbContext> options =
    //             new DbContextOptionsBuilder<IdentityAndProductDbContext>(
    //             );
    //         options.UseSqlServer(_configuration.GetConnectionString(ConnectionPropertiesConstant.MAIN_CONNECTION_STRING));
    //         await using (var dbContext = new IdentityAndProductDbContext(options.Options))
    //         {
    //             try
    //             {
    //                 var trans =  dbContext.Transaction.FirstOrDefault(e => e.Id == request.Id);
    //                 
    //                 if ((int)trans.Type == 0)
    //                 {
    //                     var purchaseOrder = await dbContext.PurchaseOrder.FirstOrDefaultAsync(e => e.TransactionId == trans.Id);
    //                     
    //                     foreach (var orderItem in purchaseOrder.PurchaseOrderProduct)
    //                         dbContext.OrderItem.Remove(orderItem);
    //                     
    //                     dbContext.PurchaseOrder.Remove(purchaseOrder);
    //                     
    //                 }
    //                 //
    //                 // else if ((int)trans.Type == 3)
    //                 // {
    //                 //     var receiptOrder = await dbContext.GoodsReceiptOrder.FirstOrDefaultAsync(e => e.TransactionId == trans.Id);
    //                 //     
    //                 //     foreach (var orderItem in receiptOrder.ReceivedOrderItems)
    //                 //         dbContext.GoodsReceiptOrderItems.Remove(orderItem);
    //                 //     
    //                 //     dbContext.GoodsReceiptOrder.Remove(
    //                 //         dbContext.GoodsReceiptOrder.FirstOrDefault(e => e.TransactionId == trans.Id));
    //                 // }
    //                 //
    //                 // else if ((int)trans.Type == 4)
    //                 // {
    //                 //     var issueOrder = await dbContext.GoodsIssueOrder.FirstOrDefaultAsync(e => e.TransactionId == trans.Id);
    //                 //     
    //                 //     foreach (var orderItem in issueOrder.GoodsIssueProducts)
    //                 //         dbContext.OrderItem.Remove(orderItem);
    //                 //     
    //                 //     dbContext.GoodsIssueOrder.Remove(
    //                 //         dbContext.GoodsIssueOrder.FirstOrDefault(e => e.TransactionId == trans.Id));
    //                 // }
    //                 //
    //                 // else if ((int)trans.Type == 5)
    //                 // {
    //                 //     var stockTakeOrder = await dbContext.StockTakeOrder.FirstOrDefaultAsync(e => e.TransactionId == trans.Id);
    //                 //     
    //                 //     foreach (var checkItem in stockTakeOrder.CheckItems)
    //                 //         dbContext.StockTakeItem.Remove(checkItem);
    //                 //     
    //                 //     dbContext.StockTakeOrder.Remove(
    //                 //         dbContext.StockTakeOrder.FirstOrDefault(e => e.TransactionId == trans.Id));
    //                 // }
    //                 //
    //                 // else if ((int)trans.Type == 6)
    //                 // {
    //                 //     var product = await dbContext.Product.FirstOrDefaultAsync(e => e.TransactionId == trans.Id);
    //                 //     
    //                 //     foreach (var productVariant in product.ProductVariants)
    //                 //         dbContext.ProductVariant.Remove(productVariant);
    //                 //     
    //                 //     dbContext.Product.Remove(
    //                 //         dbContext.Product.FirstOrDefault(e => e.TransactionId == trans.Id));
    //                 // }
    //
    //                 else
    //                 {
    //                     response.Status = false;
    //                     response.Verbose = "Unable to delete transaction with ID " + trans.Id+ ". Check if transaction is deletable";
    //                     return Ok(response);
    //                 }
    //                 
    //                 response.Status = true;
    //                 
    //                 trans.Type = TransactionType.Deleted;
    //                 await dbContext.SaveChangesAsync();
    //                 return Ok(response);
    //                 // await _transactionRepository.DeleteAsync(trans);
    //             }
    //             catch (Exception e)
    //             {
    //                 return NotFound(e);
    //                 throw;
    //             }
    //           
    //         }
    //     }
    // }
}