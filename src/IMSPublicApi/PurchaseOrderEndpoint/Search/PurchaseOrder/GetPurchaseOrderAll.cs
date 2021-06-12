﻿using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Elasticsearch.Net;
using Infrastructure.Services;
using InventoryManagementSystem.ApplicationCore.Entities;
using InventoryManagementSystem.ApplicationCore.Entities.Orders;
using InventoryManagementSystem.ApplicationCore.Entities.Orders.Status;
using InventoryManagementSystem.ApplicationCore.Entities.SearchIndex;
using InventoryManagementSystem.ApplicationCore.Interfaces;
using InventoryManagementSystem.PublicApi.AuthorizationEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nest;
using Swashbuckle.AspNetCore.Annotations;

namespace InventoryManagementSystem.PublicApi.PurchaseOrderEndpoint.Search.PurchaseOrder
{
    public class GetPurchaseOrderAll : BaseAsyncEndpoint.WithRequest<GetAllPurchaseOrderRequest>.WithResponse<GetAllPurchaseOrderResponse>
    {
        private readonly IAsyncRepository<ApplicationCore.Entities.Orders.PurchaseOrder> _asyncRepository;
        private readonly IAuthorizationService _authorizationService;
        private readonly IElasticClient _elasticClient;

        public GetPurchaseOrderAll(IAsyncRepository<ApplicationCore.Entities.Orders.PurchaseOrder> asyncRepository, IAuthorizationService authorizationService, IElasticClient elasticClient)
        {
            _asyncRepository = asyncRepository;
            _authorizationService = authorizationService;
            _elasticClient = elasticClient;
        }

        [HttpGet("api/purchaseorder/{SearchQuery}&status={Status}&page={CurrentPage}&size={SizePerPage}")]
        [SwaggerOperation(
            Summary = "Get all purchase Order",
            Description = "Get all purchase Order, {SearchQuery} = all to get all or search for individual field",
            OperationId = "po.update",
            Tags = new[] { "PurchaseOrderEndpoints" })
        ]

        public override async Task<ActionResult<GetAllPurchaseOrderResponse>> HandleAsync([FromRoute] GetAllPurchaseOrderRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            var response = new GetAllPurchaseOrderResponse();
            if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, "PurchaseOrder", UserOperations.Read))
                return Unauthorized();

            PagingOption<ApplicationCore.Entities.Orders.PurchaseOrder> pagingOption = new PagingOption<ApplicationCore.Entities.Orders.PurchaseOrder>(
                request.CurrentPage, request.SizePerPage);

            
            if (request.SearchQuery == "all")
            {
                response.IsDisplayingAll = true;
                var posi = await _asyncRepository.GetPOForELIndexAsync(cancellationToken);
                response.PurchaseOrderSearchIndices = posi.ResultList.ToList();
                
                return Ok(response);
            }
            else
            {
                var responseElastic = await _elasticClient.SearchAsync<PurchaseOrderSearchIndex>
                (
                    s => s.From(request.CurrentPage).Size(request.SizePerPage).Index("purchaseorders").Query(q => q.QueryString(d => d.Query('*' + request.SearchQuery + '*'))));
                response.PurchaseOrderSearchIndices.Clear();
                
                response.PurchaseOrderSearchIndices.AddRange(responseElastic.Documents.Where(d => d.Status == ((PurchaseOrderStatusType)request.Status).ToString()));

                // foreach (var purchaseOrderSearchIndex in responseElastic.Documents)
                // {
                //     response.PurchaseOrderSearchIndices.Add(purchaseOrderSearchIndex);
                // }
                return Ok(response);
            }
        }
    }
}