﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Elasticsearch.Net;
using Infrastructure;
using Infrastructure.Services;
using InventoryManagementSystem.ApplicationCore.Constants;
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
    // public class GetPurchaseOrderAll : BaseAsyncEndpoint.WithRequest<GetAllPurchaseOrderRequest>.WithResponse<GetAllPurchaseOrderResponse>
    // {
    //     private readonly IAsyncRepository<ApplicationCore.Entities.Orders.PurchaseOrder> _asyncRepository;
    //
    //     private readonly IAuthorizationService _authorizationService;
    //     private readonly IElasticClient _elasticClient;
    //
    //     public GetPurchaseOrderAll(IAsyncRepository<ApplicationCore.Entities.Orders.PurchaseOrder> asyncRepository, IAuthorizationService authorizationService, IElasticClient elasticClient)
    //     {
    //         _asyncRepository = asyncRepository;
    //         _authorizationService = authorizationService;
    //         _elasticClient = elasticClient;
    //     }
    //
    //     [HttpPost("api/purchaseorder/all")]
    //     [SwaggerOperation(
    //         Summary = "Get all purchase Order",
    //         Description = "Get all purchase Order"  +
    //                       "{CurrentPage}: Current page to display \n" +
    //                       "{SizePerPage}: Number of rows to display in a page \n " +
    //                       "{Status} Status of purchase order",
    //         OperationId = "po.update",
    //         Tags = new[] { "PurchaseOrderEndpoints" })
    //     ]
    //
    //     public override async Task<ActionResult<GetAllPurchaseOrderResponse>> HandleAsync(GetAllPurchaseOrderRequest request, CancellationToken cancellationToken = new CancellationToken())
    //     {
    //         var response = new GetAllPurchaseOrderResponse();
    //         if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, PageConstant.PURCHASEORDER, UserOperations.Read))
    //             return Unauthorized();
    //         
    //         PagingOption<PurchaseOrderSearchIndex> pagingOption =
    //              new PagingOption<PurchaseOrderSearchIndex>(request.CurrentPage, request.SizePerPage);
    //         response.IsDisplayingAll = true;
    //
    //         var posi = await 
    //             _asyncRepository.GetPOForELIndexAsync(pagingOption, new POSearchFilter(), cancellationToken);
    //         
    //         response.Paging = posi;
    //         
    //         
    //         return Ok(response);
    //     }
    // }
    
      public class SearchPurchaseOrder : BaseAsyncEndpoint.WithRequest<SearchPurchaseOrderRequest>.WithResponse<GetAllPurchaseOrderResponse>
    {
        private readonly IAsyncRepository<ApplicationCore.Entities.Orders.PurchaseOrder> _asyncRepository;

        private readonly IAuthorizationService _authorizationService;
        private readonly IElasticClient _elasticClient;

        public SearchPurchaseOrder(IAsyncRepository<ApplicationCore.Entities.Orders.PurchaseOrder> asyncRepository, IAuthorizationService authorizationService, IElasticClient elasticClient)
        {
            _asyncRepository = asyncRepository;
            _authorizationService = authorizationService;
            _elasticClient = elasticClient;
        }

        [HttpGet]
        [Route("api/purchaseorder/search")]
        [SwaggerOperation(
            Summary = "Search purchase Order",
            Description = "Search purchase Order"  +
                          "\n {SearchQuery}: Querry to search, all to search all \n " +
                          "{CurrentPage}: Current page to display \n" +
                          "{SizePerPage}: Number of rows to display in a page \n " +
                          "{Status} Status of purchase order",
            OperationId = "po.update",
            Tags = new[] { "PurchaseOrderEndpoints" })
        ]

        public override async Task<ActionResult<GetAllPurchaseOrderResponse>> HandleAsync([FromQuery]SearchPurchaseOrderRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            var response = new GetAllPurchaseOrderResponse();
            if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, PageConstant.PURCHASEORDER, UserOperations.Read))
                return Unauthorized();
            
            PagingOption<PurchaseOrderSearchIndex> pagingOption =
                new PagingOption<PurchaseOrderSearchIndex>(request.CurrentPage, request.SizePerPage);
            response.IsDisplayingAll = true;
            var requestUri = Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(Request);

            var poSearchFilter = new POSearchFilter
            {
                FromStatus = request.FromStatus,
                ToStatus = request.ToStatus,
                SupplierId = request.SupplierId,
                ConfirmedByName = request.ConfirmedByName,
                CreatedByName = request.CreatedByName,
                FromConfirmedDate = request.FromConfirmedDate,
                FromCreatedDate = request.FromCreatedDate,
                FromDeliveryDate = request.FromDeliveryDate,
                FromModifiedDate = request.FromModifiedDate,
                ToConfirmedDate = request.ToConfirmedDate,
                ToCreatedDate = request.ToCreatedDate,
                ToDeliveryDate = request.ToDeliveryDate,
                ToModifiedDate = request.ToModifiedDate,
                FromTotalOrderPrice = request.FromTotalOrderPrice,
                ToTotalOrderPrice = request.ToTotalOrderPrice,
            };
            if (request.SearchQuery == null)
            {
                var posi = await 
                    _asyncRepository.GetPOForELIndexAsync(pagingOption, poSearchFilter, cancellationToken);
            
                response.Paging = posi;
                return Ok(response);
            }
            
            var responseElastic = await _elasticClient.SearchAsync<PurchaseOrderSearchIndex>
            (
                s => s.Size(2000)
                    .Index(ElasticIndexConstant.PURCHASE_ORDERS).Query(q => q.QueryString(d => d.Query('*' + request.SearchQuery + '*'))));
            
            pagingOption.ResultList = _asyncRepository.PurchaseOrderIndexFiltering(responseElastic.Documents.ToList(), poSearchFilter,
                new CancellationToken());
            
            pagingOption.ExecuteResourcePaging();
            response.Paging = pagingOption;
            return Ok(response);
        }
    }
    
    
}