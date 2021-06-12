using System;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Infrastructure;
using Infrastructure.Services;
using InventoryManagementSystem.ApplicationCore.Entities.Orders.Status;
using InventoryManagementSystem.ApplicationCore.Entities.SearchIndex;
using InventoryManagementSystem.ApplicationCore.Interfaces;
using InventoryManagementSystem.PublicApi.AuthorizationEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace InventoryManagementSystem.PublicApi.PurchaseOrderEndpoint.PurchaseRequisition.Submit
{
    public class RequisitionSubmit : BaseAsyncEndpoint.WithRequest<RSubmitRequest>.WithoutResponse
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly IAsyncRepository<ApplicationCore.Entities.Orders.PurchaseOrder> _asyncRepository;
        private readonly IUserAuthentication _userAuthentication;
        private readonly IAsyncRepository<PurchaseOrderSearchIndex> _indexAsyncRepository;

        public RequisitionSubmit(IAuthorizationService authorizationService, IAsyncRepository<ApplicationCore.Entities.Orders.PurchaseOrder> asyncRepository, IUserAuthentication userAuthentication, IAsyncRepository<PurchaseOrderSearchIndex> indexAsyncRepository)
        {
            _authorizationService = authorizationService;
            _asyncRepository = asyncRepository;
            _userAuthentication = userAuthentication;
            _indexAsyncRepository = indexAsyncRepository;
        }

        [HttpPost("api/requisition/submit")]
        [SwaggerOperation(
            Summary = "Submit Requsition as role Saleman to Accountant",
            Description = "Submit Requsition as role Saleman to Accountant",
            OperationId = "r.submit",
            Tags = new[] { "RequisitionEndpoints" })
        ]
        public override async Task<ActionResult> HandleAsync(RSubmitRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, "Requisition", UserOperations.Update))
                return Unauthorized();
            
            var po = await _asyncRepository.GetByIdAsync(request.Id);

            po.Transaction.ModifiedDate = DateTime.Now;
            po.Transaction.ModifiedById = (await _userAuthentication.GetCurrentSessionUser()).Id;
            po.PurchaseOrderStatus = PurchaseOrderStatusType.RequisitionSent;

            await _asyncRepository.UpdateAsync(po);
            await _indexAsyncRepository.ElasticSaveSingleAsync(false, IndexingHelper.PurchaseOrderSearchIndex(po));
            
            return Ok();
        }
    }
}