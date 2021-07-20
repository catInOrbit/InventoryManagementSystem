﻿using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Infrastructure.Services;
using InventoryManagementSystem.ApplicationCore.Constants;
using InventoryManagementSystem.ApplicationCore.Entities;
using InventoryManagementSystem.ApplicationCore.Entities.SearchIndex;
using InventoryManagementSystem.ApplicationCore.Interfaces;
using InventoryManagementSystem.PublicApi.AuthorizationEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nest;
using Swashbuckle.AspNetCore.Annotations;

namespace InventoryManagementSystem.PublicApi.PurchaseOrderEndpoint.Search.PurchaseOrder
{
    
      public class SearchPurchaseOrder : BaseAsyncEndpoint.WithRequest<SearchPurchaseOrderRequest>.WithResponse<GetAllPurchaseOrderResponse>
    {
        private readonly IAsyncRepository<ApplicationCore.Entities.Orders.PurchaseOrder> _asyncRepository;

        private readonly IAuthorizationService _authorizationService;
        private readonly IUserSession _userSession;

        private readonly IElasticClient _elasticClient;

        public SearchPurchaseOrder(IAsyncRepository<ApplicationCore.Entities.Orders.PurchaseOrder> asyncRepository, IAuthorizationService authorizationService, IElasticClient elasticClient, IUserSession userSession)
        {
            _asyncRepository = asyncRepository;
            _authorizationService = authorizationService;
            _elasticClient = elasticClient;
            _userSession = userSession;
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
                HideMerged = request.HideMerged,
                IgnoreOrderIds = request.IgnoreOrderID
            };
            
            ISearchResponse<PurchaseOrderSearchIndex> responseElastic;

            ElasticSearchHelper<PurchaseOrderSearchIndex> elasticSearchHelper = new ElasticSearchHelper<PurchaseOrderSearchIndex>(_elasticClient, request.SearchQuery,
                ElasticIndexConstant.PURCHASE_ORDERS);
            responseElastic = await elasticSearchHelper.SearchDocuments();
            
            // if (request.SearchQuery == null)
            // {
            //     var posi = await 
            //         _asyncRepository.GetPOForELIndexAsync(request.HideMerged, pagingOption, poSearchFilter, cancellationToken);
            //
            //     response.Paging = posi;
            //     return Ok(response);
            // }
            //
            // var responseElastic = await _elasticClient.SearchAsync<PurchaseOrderSearchIndex>
            // (
            //     s => s.Size(2000)
            //         .Index(ElasticIndexConstant.PURCHASE_ORDERS).Query(q => q.QueryString(d => d.Query('*' + request.SearchQuery + '*'))));
            
            pagingOption.ResultList = _asyncRepository.PurchaseOrderIndexFiltering(responseElastic.Documents.ToList(), poSearchFilter,
                new CancellationToken());
            
            pagingOption.ExecuteResourcePaging();
            response.Paging = pagingOption;
            return Ok(response);
        }
    }
    
    
}