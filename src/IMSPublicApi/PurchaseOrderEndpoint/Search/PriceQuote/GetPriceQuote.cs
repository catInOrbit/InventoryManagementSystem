﻿using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Infrastructure.Services;
using InventoryManagementSystem.ApplicationCore.Entities.Orders;
using InventoryManagementSystem.ApplicationCore.Entities.Orders.Status;
using InventoryManagementSystem.ApplicationCore.Entities.SearchIndex;
using InventoryManagementSystem.ApplicationCore.Interfaces;
using InventoryManagementSystem.PublicApi.AuthorizationEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace InventoryManagementSystem.PublicApi.PurchaseOrderEndpoint.Search.PriceQuote
{
    public class GetPriceQuote : BaseAsyncEndpoint.WithRequest<GetPriceQuoteRequest>.WithResponse<GetPriceQuoteResponse>
    {
        private IAsyncRepository<PriceQuoteOrder> _asyncRepository;
        private readonly IAuthorizationService _authorizationService;

        public GetPriceQuote(IAsyncRepository<PriceQuoteOrder> asyncRepository, IAuthorizationService authorizationService)
        {
            _asyncRepository = asyncRepository;
            _authorizationService = authorizationService;
        }
        
        [HttpGet("api/pricequote/{number}")]
        [SwaggerOperation(
            Summary = "Get all price quote",
            Description = "Get all price quote",
            OperationId = "po.update",
            Tags = new[] { "PriceQuoteOrderEndpoints" })
        ]
        public override async Task<ActionResult<GetPriceQuoteResponse>> HandleAsync([FromRoute] GetPriceQuoteRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            // var isAuthorized = await _authorizationService.AuthorizeAsync(
            //     HttpContext.User, "PurchaseOrder",
            //     UserOperations.Read);
            //
            // if (!isAuthorized.Succeeded)
            //     return Unauthorized();
            
            if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, "PriceQuoteOrder", UserOperations.Read))
                return Unauthorized();
            
            var response = new GetPriceQuoteResponse();

            if (request.number == "all")
            {
                var pqrs = await _asyncRepository.ListAllAsync(cancellationToken);
                foreach (var priceQuoteOrder in pqrs)
                {
                    if (priceQuoteOrder.PriceQuoteStatus == PriceQuoteType.Pending)
                    {
                        var pq = new PQDisplay
                        {
                            Id = priceQuoteOrder.Id,
                            Deadline = priceQuoteOrder.Deadline.ToString("MM/dd/yyyy"),
                            CreatedDate = priceQuoteOrder.Transaction.CreatedDate.ToString("MM/dd/yyyy"),
                            CreatedByName = priceQuoteOrder.Transaction.CreatedBy.Fullname,
                            PriceQuoteOrderNumber = priceQuoteOrder.PriceQuoteNumber,
                            SupplierName = (priceQuoteOrder.Supplier != null) ? priceQuoteOrder.Supplier.SupplierName : "",
                        };
                        
                        foreach (var purchaseOrderItem in priceQuoteOrder.PurchaseOrderProduct)
                        {
                            pq.NumberOfProduct = (float)Math.Round(pq.NumberOfProduct + 1) ;
                        }
                        response.PriceQuotes.Add(pq);     
                    }
                }
            }

            else
            {
                // var pos = await _asyncRepository.ListAllAsync(cancellationToken);
                // var responseElastic = await _elasticClient.SearchAsync<PurchaseOrderSearchIndex>(
                //     s => s.Query(q => q.QueryString(d => d.Query('*' + request.number + '*'))));
                var pqr = _asyncRepository.GetPriceQuoteByNumber(request.number, cancellationToken);
                response.PriceQuoteOrders.Clear();
                response.PriceQuoteOrders.Add(pqr);    
            }
            return Ok(response);
        }
    }
}