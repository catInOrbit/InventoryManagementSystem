using System;
using System.Collections.Generic;
using InventoryManagementSystem.ApplicationCore.Entities;
using InventoryManagementSystem.ApplicationCore.Entities.Orders;
using InventoryManagementSystem.ApplicationCore.Entities.SearchIndex;
using InventoryManagementSystem.ApplicationCore.Extensions;
using Newtonsoft.Json;

namespace InventoryManagementSystem.PublicApi.PurchaseOrderEndpoint
{
    public class PQCreateRequest : BaseRequest
    {
        public string Id { get; set; }
    }
    
    public class PQCreateResponse : BaseResponse
    {
        public ApplicationCore.Entities.Orders.PurchaseOrder PurchaseOrder { get; set; }
        public List<MergedOrderIdList> MergedOrderIdLists { get; set; }
    }
    
    public class PQEditRequest : BaseRequest 
    {
        // public PriceQuoteOrder PriceQuoteOrder { get; set; }
        
        public string PurchaseOrderNumber { get; set; }
        public string SupplierId { get; set; }
        public List<string> MergedRequisitionIds { get; set; }
        public DateTime Deadline { get; set; }
        public string MailDescription { get; set; }
        public ICollection<OrderItem> OrderItemInfos  { get; set; }
    }
    
    public class PQEditResponse : BaseResponse
    {
        public PQEditResponse(Guid correlationId) : base(correlationId)
        { }

        public PQEditResponse()
        { }
        
        public ApplicationCore.Entities.Orders.PurchaseOrder PurchaseOrder { get; set; }
        public List<MergedOrderIdList> MergedOrderIdLists { get; set; }
        public bool Result { get; set; }
    }
    
    public class PQSubmitRequest : BaseRequest
    {
        public string OrderNumber { get; set; }
    }
    
    public class PQSubmitResponse : BaseResponse
    {
        public ApplicationCore.Entities.Orders.PurchaseOrder PurchaseOrder { get; set; }
    }
    
    public class POConfirmRequest : BaseRequest
    {
        public string PurchaseOrderNumber { get; set; }
    }
    
    public class POConfirmResponse : BaseResponse
    {
        public ApplicationCore.Entities.Orders.PurchaseOrder PurchaseOrder { get; set; }
    }
    
    public class POCreateRequest : BaseRequest
    {
        public string PurchaseOrderNumber { get; set; }
    }
    public class POCreateResponse : BaseResponse
    {
        public POCreateResponse(Guid correlationId) : base(correlationId)
        {
            
        }

        public POCreateResponse()
        {
            
        }

        public ApplicationCore.Entities.Orders.PurchaseOrder PurchaseOrder { get; set; }
        
    }
    
    public class PORejectRequest : BaseRequest
    {
        public string Id { get; set; }
        public string CancelReason { get; set; }
    }
    
    public class PORejectResponse : BaseResponse
    {
        public ApplicationCore.Entities.Orders.PurchaseOrder PurchaseOrder { get; set; }
    }
    
    public class POSubmitRequest : BaseRequest
    {
        public string PurchaseOrderNumber { get; set; }
    }
    
    public class POSubmitResponse : BaseResponse
    {
        public POSubmitResponse(Guid correlationId) : base(correlationId)
        {
            
        }

        public POSubmitResponse()
        {
            
        }
        public ApplicationCore.Entities.Orders.PurchaseOrder PurchaseOrder { get; set; }
    }
    
    public class POUpdateRequest : BaseRequest
    {
        public string PurchaseOrderNumber { get; set; }
        public string MailDescription { get; set; }
        public ICollection<OrderItem> OrderItemInfos  { get; set; }
    }
    
    public class POUpdateResponse : BaseResponse
    {
        public POUpdateResponse(Guid correlationId) : base(correlationId)
        { }

        public POUpdateResponse()
        { }

        public bool Result { get; set; }
        public string Verbose { get; set; }
        public ApplicationCore.Entities.Orders.PurchaseOrder PurchaseOrder { get; set; }

    }
    
    public class RCreateResponse : BaseResponse
    {
        public ApplicationCore.Entities.Orders.PurchaseOrder PurchaseOrder { get; set; } =
            new ApplicationCore.Entities.Orders.PurchaseOrder();
    }
    
    public class RSubmitRequest : BaseRequest
    {
        public string Id { get; set; }
    }
    
    public class RequisitionUpdateResponse : BaseResponse
    {
        public string TransactionId { get; set; }
        public string UpdatedRequisitionId { get; set; }
    }
    
    public class RequisitionCreateResponse : BaseResponse
    {
        public string TransactionId { get; set; }
        public string CreatedRequisitionId { get; set; }
    }
    
    public class RequisitionCreateRequest
    {
        public DateTime Deadline { get; set; }
        public List<OrderItem> OrderItems { get; set; }
    }
    
    public class RequisitionUpdateRequest
    {
        public string RequisitionId { get; set; }
        public DateTime Deadline { get; set; }
        public List<OrderItem> OrderItems { get; set; }
    }

    
   public class GetPriceQuoteRequest : BaseRequest
    {
        public string Number { get; set; }
        
        public int CurrentPage { get; set; }
        public int SizePerPage { get; set; }
    }
   
   public class GetPriceQuoteResponse : BaseResponse
   {
       public GetPriceQuoteResponse(Guid correlationId) : base(correlationId)
       {
                
       }

       public GetPriceQuoteResponse()
       {
                
       }
       public virtual bool ShouldSerializePriceQuoteOrder()
       {
           if(!IsForDisplay)
               return true;
           return false;
       }
        
        
       public virtual bool ShouldSerializePriceQuotes()
       {
           if(IsForDisplay)
               return true;
           return false;
       }
        
       public ApplicationCore.Entities.Orders.PurchaseOrder PriceQuoteOrder { get; set; } = new ApplicationCore.Entities.Orders.PurchaseOrder();
       public List<PqDisplay> PriceQuotes { get; set; } = new List<PqDisplay>();

       [JsonIgnore]
       public bool IsForDisplay { get; set; }
   }
   
   public class GetAllPurchaseOrderResponse : BaseResponse
   {
       [JsonIgnore]
       public bool IsDisplayingAll { get; set; }
        
       // public List<PurchaseOrderSearchIndex> PurchaseOrderSearchIndices { get; set; } =
       //     new List<PurchaseOrderSearchIndex>();
       public PagingOption<PurchaseOrderSearchIndex> Paging { get; set; }
       public ApplicationCore.Entities.Orders.PurchaseOrder PurchaseOrder { get; set; }
       public List<MergedOrderIdList> MergedOrderIdLists { get; set; }
   }
   
   public class GetPurchaseOrderIdRequest
   {
       public string Id { get; set; }
   }
   
  public class GetAllPurchaseOrderRequest : BaseRequest
   {
       public int CurrentPage { get; set; }
       public int SizePerPage { get; set; }
       public POSearchFilter PoSearchFilter { get; set; }
   }
  
  public class SearchPurchaseOrderRequest : POSearchFilter
  {
      public string SearchQuery { get; set; }
      public int CurrentPage { get; set; }
      public int SizePerPage { get; set; }
      
    
  }
  
  public class POResponse : BaseResponse
  {
      public ApplicationCore.Entities.Orders.PurchaseOrder PurchaseOrder { get; set; }
  }
      
}