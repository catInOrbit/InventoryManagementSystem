using System;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Infrastructure.Data;
using InventoryManagementSystem.ApplicationCore.Entities;
using InventoryManagementSystem.ApplicationCore.Entities.RequestAndForm;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagementSystem.PublicApi.PurchaseOrderEndpoint
{
    public class PurchaseOrder : BaseAsyncEndpoint.WithRequest<PurchaseOrderRequest>.WithResponse<PurchaseRequestResponse>
    {
        private readonly EfRepository<Transaction> _efRepositoryTransaction;
        private readonly EfRepository<Supplier> _efRepositorySupplier;

        public PurchaseOrder(EfRepository<Transaction> efRepository, EfRepository<Supplier> efRepositorySupplier)
        {
            _efRepositoryTransaction = efRepository;
            _efRepositorySupplier = efRepositorySupplier;
        }

        public override async Task<ActionResult<PurchaseRequestResponse>> HandleAsync(PurchaseOrderRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            var response = new PurchaseRequestResponse();
            var supplier = await _efRepositorySupplier.GetByIdAsync(request.SupplierID, cancellationToken);

            var transaction = new Transaction
            {
                Name = "PurchaseOrder " + supplier.SupplierName + " " + DateTime.Now,
                SupplierId = request.SupplierID
                
            };
            _efRepository.AddAsync(request.SupplierID);
        }
    }
}