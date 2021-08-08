using System.Collections.Generic;
using InventoryManagementSystem.ApplicationCore.Entities.Orders;
using InventoryManagementSystem.ApplicationCore.Extensions;

namespace InventoryManagementSystem.PublicApi.EmailEndpoints
{
    public class MailSendingResponse : BaseResponse
    {
        public bool Result { get; set; }
    }
    
    public class MailSendingPOResponse : BaseResponse
    {
        public PurchaseOrder PurchaseOrder { get; set; }
        public List<MergedOrderIdList> MergedOrderIdLists { get; set; }
    }
}