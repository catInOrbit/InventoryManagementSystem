using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using InventoryManagementSystem.ApplicationCore.Entities.Orders;
using InventoryManagementSystem.ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace InventoryManagementSystem.PublicApi.TransactionGeneral
{
    public class DeleteTransaction : BaseAsyncEndpoint.WithRequest<DeleteTransactionRequest>.WithoutResponse
    {
        private IAsyncRepository<Transaction> _transactionRepository;

        public DeleteTransaction(IAsyncRepository<Transaction> transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }

        [HttpDelete("api/transaction/{Id}")]
        [SwaggerOperation(
            Summary = "Delete a transaction (Change status to false)",
            Description = "Delete a transaction (Change status to false)",
            OperationId = "transac.delete",
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
}