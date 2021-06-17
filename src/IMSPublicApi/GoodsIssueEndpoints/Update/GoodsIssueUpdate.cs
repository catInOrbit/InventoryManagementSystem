using System;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Infrastructure.Services;
using InventoryManagementSystem.ApplicationCore.Entities.Orders;
using InventoryManagementSystem.ApplicationCore.Entities.Orders.Status;
using InventoryManagementSystem.ApplicationCore.Interfaces;
using InventoryManagementSystem.PublicApi.AuthorizationEndpoints;
using InventoryManagementSystem.PublicApi.GoodsIssueEndpoints.Create;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace InventoryManagementSystem.PublicApi.GoodsIssueEndpoints.Update
{
    public class GoodsIssueUpdate : BaseAsyncEndpoint.WithRequest<GiRequest>.WithResponse<GiResponse>
    {
        private readonly IUserAuthentication _userAuthentication;
        private readonly IAsyncRepository<GoodsIssueOrder> _asyncRepository;
        private readonly IAuthorizationService _authorizationService;

        public GoodsIssueUpdate(IUserAuthentication userAuthentication, IAsyncRepository<GoodsIssueOrder> asyncRepository, IAuthorizationService authorizationService)
        {
            _userAuthentication = userAuthentication;
            _asyncRepository = asyncRepository;
            _authorizationService = authorizationService;
        }

        [HttpPost("api/goodsissue/update")]
        [SwaggerOperation(
            Summary = "Update Good issue order",
            Description = "Update Good issue order",
            OperationId = "gio.update",
            Tags = new[] { "GoodsIssueEndpoints" })
        ]
        public override async Task<ActionResult<GiResponse>> HandleAsync(GiRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, "GoodsIssue", UserOperations.Update))
                return Unauthorized();
            
            var gio = _asyncRepository.GetGoodsIssueOrderByNumber(request.IssueNumber);
            switch (request.ChangeStatusTo)
            {
                case "Shipping":
                    gio.GoodsIssueType = GoodsIssueStatusType.Shipping;
                    break;
                case "Confirm":
                    gio.GoodsIssueType = GoodsIssueStatusType.Completed;
                    break;
            }
            gio.GoodsIssueType = GoodsIssueStatusType.Shipping;
            gio.Transaction.ModifiedDate = DateTime.Now;
            gio.Transaction.ModifiedById = (await _userAuthentication.GetCurrentSessionUser()).Id;
            await _asyncRepository.UpdateAsync(gio);

            var response = new GiResponse();
            response.GoodsIssueOrder = gio;
            return Ok(response);
        }
    }
}