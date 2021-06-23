using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Infrastructure.Data;
using Infrastructure.Identity.DbContexts;
using InventoryManagementSystem.ApplicationCore.Constants;
using InventoryManagementSystem.ApplicationCore.Entities.Orders;
using InventoryManagementSystem.ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Swashbuckle.AspNetCore.Annotations;

namespace InventoryManagementSystem.PublicApi.TransactionGeneral
{
    public class DeactivateTransaction : BaseAsyncEndpoint.WithRequest<DeleteTransactionRequest>.WithoutResponse
    {
        private IAsyncRepository<Transaction> _transactionRepository;

        public DeactivateTransaction(IAsyncRepository<Transaction> transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }

        [HttpPut("api/transaction/{Id}")]
        [SwaggerOperation(
            Summary = "Deactivate a transaction (Change status to false)",
            Description = "Deactivate a transaction (Change status to false)",
            OperationId = "transac.deactivate",
            Tags = new[] { "TransactionEndpoints" })
        ]
        public override async Task<ActionResult> HandleAsync([FromRoute] DeleteTransactionRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            var trans = await _transactionRepository.GetByIdAsync(request.Id);
            trans.TransactionStatus = false;

            await _transactionRepository.UpdateAsync(trans);
            return Ok();
        }
    }
    
    // public class DeleteTransaction : BaseAsyncEndpoint.WithRequest<DeleteTransactionRequest>.WithoutResponse
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
    //     public override async Task<ActionResult> HandleAsync([FromRoute] DeleteTransactionRequest request, CancellationToken cancellationToken = new CancellationToken())
    //     {
    //         
    //         DbContextOptionsBuilder<IdentityAndProductDbContext> options =
    //             new DbContextOptionsBuilder<IdentityAndProductDbContext>(
    //             );
    //         options.UseSqlServer(_configuration.GetConnectionString(ConnectionPropertiesConstant.MAIN_CONNECTION_STRING));
    //         await using (var dbContext = new IdentityAndProductDbContext(options.Options))
    //         {
    //             var trans =  dbContext.Transaction.FirstOrDefault(e => e.Id == request.Id);
    //             
    //             if (request.Type >= 0 && request.Type <= 2)
    //             {
    //                 dbContext.PurchaseOrder.Remove(
    //                     dbContext.PurchaseOrder.FirstOrDefault(e => e.TransactionId == trans.Id));
    //             }
    //             
    //             if (request.Type == 3)
    //             {
    //                 dbContext.GoodsReceiptOrder.Remove(
    //                     dbContext.GoodsReceiptOrder.FirstOrDefault(e => e.TransactionId == trans.Id));
    //             }
    //
    //             await dbContext.SaveChangesAsync();
    //             Console.WriteLine(trans.GoodsReceiptOrder.Id);
    //             // await _transactionRepository.DeleteAsync(trans);
    //         }
    //         return Ok();
    //     }
    // }
}