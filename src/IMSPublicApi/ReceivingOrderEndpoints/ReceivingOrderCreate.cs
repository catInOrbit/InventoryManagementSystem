﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Infrastructure.Services;
using InventoryManagementSystem.ApplicationCore.Entities.Orders;
using InventoryManagementSystem.ApplicationCore.Entities.Orders.Status;
using InventoryManagementSystem.ApplicationCore.Entities.RequestAndForm;
using InventoryManagementSystem.ApplicationCore.Interfaces;
using InventoryManagementSystem.PublicApi.AuthorizationEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace InventoryManagementSystem.PublicApi.ReceivingOrderEndpoints
{
    public class ReceivingOrderCreate : BaseAsyncEndpoint.WithoutRequest.WithResponse<ROCreateResponse>
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly IAsyncRepository<GoodsReceiptOrder> _receiveAsyncRepository;
        private readonly IUserAuthentication _userAuthentication;

  
        public ReceivingOrderCreate(IAuthorizationService authorizationService, IAsyncRepository<GoodsReceiptOrder> receiveAsyncRepository, IUserAuthentication userAuthentication)
        {
            _authorizationService = authorizationService;
            _receiveAsyncRepository = receiveAsyncRepository;
            _userAuthentication = userAuthentication;
        }
        
              
        [HttpPost("api/receiving/create")]
        [SwaggerOperation(
            Summary = "Update Receiving Order",
            Description = "Update Receiving Order",
            OperationId = "ro.update",
            Tags = new[] { "ReceivingOrderEndpoints" })
        ]
        public override async Task<ActionResult<ROCreateResponse>> HandleAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            if(! await UserAuthorizationService.Authorize(_authorizationService, HttpContext.User, "Product", UserOperations.Read))
                return Unauthorized();
            
            
            var ro = new GoodsReceiptOrder();

            var transaction = new Transaction
            {
                TransactionId = ro .Id,
                TransactionNumber = DateTime.UtcNow.Date.ToString("ddMMyyyy") + Guid.NewGuid().ToString().Substring(0, 5).ToUpper(),
                CreatedDate = DateTime.Now,
                Type = TransactionType.Purchase,
                CreatedById = (await _userAuthentication.GetCurrentSessionUser()).Id
            };

            ro.Transaction = transaction;
            await _receiveAsyncRepository.AddAsync(ro);
            await _receiveAsyncRepository.ElasticSaveSingleAsync(ro);

            var response = new ROCreateResponse();
            response.ReceivingOrder = ro;
            return Ok(response);
        }
    }
}