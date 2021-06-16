using System;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Infrastructure;
using Infrastructure.Services;
using InventoryManagementSystem.ApplicationCore.Entities.Products;
using InventoryManagementSystem.ApplicationCore.Entities.SearchIndex;
using InventoryManagementSystem.ApplicationCore.Interfaces;
using InventoryManagementSystem.PublicApi.AuthorizationEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace InventoryManagementSystem.PublicApi.PurchaseOrderEndpoint.PurchaseRequisition.Update
{
    public class RequisitionUpdate : BaseAsyncEndpoint.WithRequest<RUpdateRequest>.WithResponse<RUpdateResponse>
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly IAsyncRepository<ApplicationCore.Entities.Orders.PurchaseOrder> _asyncRepository;
        private readonly IAsyncRepository<ProductVariant> _pvasyncRepository;
        private readonly IUserAuthentication _userAuthentication;
        private readonly IAsyncRepository<PurchaseOrderSearchIndex> _indexAsyncRepository;

        public RequisitionUpdate(IAuthorizationService authorizationService, IAsyncRepository<ApplicationCore.Entities.Orders.PurchaseOrder> asyncRepository, IUserAuthentication userAuthentication, IAsyncRepository<PurchaseOrderSearchIndex> indexAsyncRepository, IAsyncRepository<ProductVariant> pvasyncRepository)
        {
            _authorizationService = authorizationService;
            _asyncRepository = asyncRepository;
            _userAuthentication = userAuthentication;
            _indexAsyncRepository = indexAsyncRepository;
            _pvasyncRepository = pvasyncRepository;
        }

        [HttpPut("api/requisition/update")]
        [SwaggerOperation(
            Summary = "Update Requsition as role Saleman",
            Description = "Submit Requsition as role Saleman ",
            OperationId = "r.update",
            Tags = new[] { "RequisitionEndpoints" })
        ]

        public override async Task<ActionResult<RUpdateResponse>> HandleAsync(RUpdateRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, "Requisition", UserOperations.Update))
                return Unauthorized();
            var po = await _asyncRepository.GetByIdAsync(request.Id);

            po.PurchaseOrderProduct.Clear();
            foreach (var requestOrderItem in request.OrderItems)
            {
                requestOrderItem.OrderId = po.Id;
                requestOrderItem.ProductVariant = await _pvasyncRepository.GetByIdAsync(requestOrderItem.ProductVariantId);
            }
            
            po.PurchaseOrderProduct = request.OrderItems;
            
            po.Transaction.ModifiedDate = DateTime.Now;
            po.Transaction.ModifiedById = (await _userAuthentication.GetCurrentSessionUser()).Id;
            po.Deadline = request.Deadline;
            
            await _asyncRepository.UpdateAsync(po);
            await _indexAsyncRepository.ElasticSaveSingleAsync(false, IndexingHelper.PurchaseOrderSearchIndex(po));
            
            
            return Ok(new RUpdateResponse{PurchaseOrder = po});
        }
    }
}