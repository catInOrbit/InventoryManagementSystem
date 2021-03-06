using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Infrastructure.Services;
using InventoryManagementSystem.ApplicationCore.Constants;
using InventoryManagementSystem.ApplicationCore.Entities;
using InventoryManagementSystem.ApplicationCore.Entities.Orders;
using InventoryManagementSystem.ApplicationCore.Entities.Orders.Status;
using InventoryManagementSystem.ApplicationCore.Entities.SearchIndex;
using InventoryManagementSystem.ApplicationCore.Interfaces;
using InventoryManagementSystem.ApplicationCore.Services;
using InventoryManagementSystem.PublicApi.AuthorizationEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nest;
using Swashbuckle.AspNetCore.Annotations;

namespace InventoryManagementSystem.PublicApi.ReceivingOrderEndpoints.Search
{
  //  public class GetReceivingOrder : BaseAsyncEndpoint.WithRequest<ROGetAllRequest>.WithResponse<ROGetResponse>
  // {
  //     private IAuthorizationService _authorizationService;
  //     private IAsyncRepository<GoodsReceiptOrder> _asyncRepository;
  //     private readonly IElasticClient _elasticClient;
  //
  //     public GetReceivingOrder(IAuthorizationService authorizationService, IAsyncRepository<GoodsReceiptOrder> asyncRepository, IElasticClient elasticClient)
  //     {
  //         _authorizationService = authorizationService;
  //         _asyncRepository = asyncRepository;
  //         _elasticClient = elasticClient;
  //     }
  //     
  //     
  //     [HttpPost("api/goodsreceipt/all")]
  //     [SwaggerOperation(
  //         Summary = "Get all receive Order",
  //         Description = "Get all Receive Order"+
  //                       "\n {Query}: Querry to search, all to search all \n " +
  //                       "{CurrentPage}: Current page to display \n" +
  //                       "{SizePerPage}: Number of rows to display in a page" ,
  //         OperationId = "ro.search",
  //         Tags = new[] { "GoodsReceiptOrders" })
  //     ]
  //     public override async Task<ActionResult<ROGetResponse>> HandleAsync(ROGetAllRequest request, CancellationToken cancellationToken = new CancellationToken())
  //     {
  //         
  //         // if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, PageConstant.GOODSRECEIPT, UserOperations.Read))
  //             // return Unauthorized();
  //         
  //         PagingOption<GoodsReceiptOrderSearchIndex> pagingOption = new PagingOption<GoodsReceiptOrderSearchIndex>(
  //             request.CurrentPage, request.SizePerPage);
  //
  //         var response = new ROGetResponse();
  //         response.IsDislayingAll = true;
  //         
  //         response.Paging  = await _asyncRepository.GetROForELIndexAsync(pagingOption, request.RoSearchFilter, cancellationToken);
  //
  //         return Ok(response);
  //     }
  // }
  //  
    public class SearchReceivingOrder : BaseAsyncEndpoint.WithRequest<ROSearchRequest>.WithResponse<ROGetResponse>
      {
          private IAuthorizationService _authorizationService;
          private IAsyncRepository<GoodsReceiptOrder> _asyncRepository;
          private readonly IElasticClient _elasticClient;
  
          public SearchReceivingOrder(IAuthorizationService authorizationService, IAsyncRepository<GoodsReceiptOrder> asyncRepository, IElasticClient elasticClient)
          {
              _authorizationService = authorizationService;
              _asyncRepository = asyncRepository;
              _elasticClient = elasticClient;
          }
          
          
          [HttpGet("api/goodsreceipt/search")]
          [SwaggerOperation(
              Summary = "Get all receive Order",
              Description = "Get all Receive Order"+
                            "\n {Query}: Querry to search, all to search all \n " +
                            "{CurrentPage}: Current page to display \n" +
                            "{SizePerPage}: Number of rows to display in a page" ,
              OperationId = "ro.searchall",
              Tags = new[] { "GoodsReceiptOrders" })
          ]
          public override async Task<ActionResult<ROGetResponse>> HandleAsync([FromQuery]ROSearchRequest request, CancellationToken cancellationToken = new CancellationToken())
          {
              
              if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, PageConstant.GOODSRECEIPT, UserOperations.Read))
                  return Unauthorized();
              
              PagingOption<GoodsReceiptOrderSearchIndex> pagingOption = new PagingOption<GoodsReceiptOrderSearchIndex>(
                  request.CurrentPage, request.SizePerPage);
              
              var response = new ROGetResponse();
              response.IsDislayingAll = true;

              var roSearchFilter = new ROSearchFilter
              {
                  CreatedByName = request.CreatedByName,
                  FromCreatedDate = request.FromCreatedDate,
                  ToCreatedDate = request.ToCreatedDate,
                  SupplierName = request.SupplierName
              };
              
              
              ISearchResponse<GoodsReceiptOrderSearchIndex> responseElastic;
      
              ElasticSearchHelper<GoodsReceiptOrderSearchIndex> elasticSearchHelper = new ElasticSearchHelper<GoodsReceiptOrderSearchIndex>(_elasticClient, request.SearchQuery,
                  ElasticIndexConstant.RECEIVING_ORDERS);
              responseElastic = await elasticSearchHelper.GetDocuments();


              pagingOption.ResultList = FilteringService.ReceivingOrderIndexFiltering(
                  responseElastic.Documents.ToList(), roSearchFilter, cancellationToken);
                              
              pagingOption.ExecuteResourcePaging();

              response.Paging = pagingOption;
  
              return Ok(response);
          }
      }
   
   public class GetReceivingOrderById : BaseAsyncEndpoint.WithRequest<ROIdGetRequest>.WithResponse<ROGetResponse>
   {
       private readonly IAsyncRepository<GoodsReceiptOrder> _receivingOrderAsyncRepository;
       private readonly IAuthorizationService _authorizationService;

       public GetReceivingOrderById(IAuthorizationService authorizationService, IAsyncRepository<GoodsReceiptOrder> receivingOrderAsyncRepository)
       {
           _authorizationService = authorizationService;
           _receivingOrderAsyncRepository = receivingOrderAsyncRepository;
       }

       [HttpGet("api/goodsreceipt/id/{Id}")]
       [SwaggerOperation(
           Summary = "Get specific receive Order",
           Description = "Get specific receive Order",
           OperationId = "ro.searchid",
           Tags = new[] { "GoodsReceiptOrders" })
       ]
       public override async Task<ActionResult<ROGetResponse>> HandleAsync([FromRoute]ROIdGetRequest request, CancellationToken cancellationToken = new CancellationToken())
       {
           if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, PageConstant.GOODSRECEIPT, UserOperations.Read))
               return Unauthorized();
            
           var response = new ROGetResponse();
           response.IsDislayingAll = false;

           response.ReceiveingOrder = await _receivingOrderAsyncRepository.GetByIdAsync(request.Id);
           foreach (var receiveingOrderReceivedOrderItem in response.ReceiveingOrder.ReceivedOrderItems)
           {
               receiveingOrderReceivedOrderItem.ProductVariantName =
                   receiveingOrderReceivedOrderItem.ProductVariant.Name;
               receiveingOrderReceivedOrderItem.VariantSku = receiveingOrderReceivedOrderItem.ProductVariant.Sku;
               receiveingOrderReceivedOrderItem.VariantBarcode =
                   receiveingOrderReceivedOrderItem.ProductVariant.Barcode;
           }
           return Ok(response);
       }
   }
   
   // public class SearchPurchaseOrdersForGoodReceipt : BaseAsyncEndpoint.WithRequest<POsForRORequest>.WithResponse<POsForROResponse>
   // {
   //     private readonly IAuthorizationService _authorizationService;
   //     private readonly IElasticClient _elasticClient;
   //
   //     public SearchPurchaseOrdersForGoodReceipt(IAuthorizationService authorizationService, IElasticClient elasticClient)
   //     {
   //         _authorizationService = authorizationService;
   //         _elasticClient = elasticClient;
   //     }
   //
   //     [HttpGet("api/goodsreceipt/searchpoids")]
   //     [SwaggerOperation(
   //         Summary = "Search Ids of confirmed Purchase Order for a Goods Receipt",
   //         Description = "Search Ids of confirmed Purchase Order for a Goods Receipt",
   //         OperationId = "ro.getpoids",
   //         Tags = new[] { "GoodsReceiptOrders" })
   //     ]
   //
   //     public override async Task<ActionResult<POsForROResponse>> HandleAsync(POsForRORequest request, CancellationToken cancellationToken = new CancellationToken())
   //     {
   //         if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, PageConstant.GOODSRECEIPT, UserOperations.Read))
   //             return Unauthorized();
   //          
   //         var response = new POsForROResponse();
   //         
   //                     
   //         var responseElastic = await _elasticClient.SearchAsync<PurchaseOrderSearchIndex>
   //         (
   //             s => s.Size(2000).Index(ElasticIndexConstant.PURCHASE_ORDERS).Query(q => q.QueryString(d => d.Query('*' + request.Id + '*'))));
   //
   //         var poIndexList =
   //             responseElastic.Documents.Where(d => d.Status == PurchaseOrderStatusType.POConfirm.ToString());
   //         foreach (var purchaseOrderSearchIndex in poIndexList)
   //             response.PurchaseOrderIdList.Add(purchaseOrderSearchIndex.Id);
   //         
   //         return Ok(response);
   //     }
   // }
   
}