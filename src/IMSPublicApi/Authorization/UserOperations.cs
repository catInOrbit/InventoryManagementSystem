using Infrastructure.Data;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace InventoryManagementSystem.PublicApi.Authorization
{
    public static class UserOperations
    {
        public static OperationAuthorizationRequirement Create =   
          new OperationAuthorizationRequirement {Name=AuthenticationConstants.CreateOperationName};
        public static OperationAuthorizationRequirement Read = 
          new OperationAuthorizationRequirement {Name=AuthenticationConstants.ReadOperationName};  
        public static OperationAuthorizationRequirement Update = 
          new OperationAuthorizationRequirement {Name=AuthenticationConstants.UpdateOperationName}; 
        public static OperationAuthorizationRequirement Delete = 
          new OperationAuthorizationRequirement {Name=AuthenticationConstants.DeleteOperationName};
        public static OperationAuthorizationRequirement Approve = 
          new OperationAuthorizationRequirement {Name=AuthenticationConstants.ApproveOperationName};
        public static OperationAuthorizationRequirement Reject = 
          new OperationAuthorizationRequirement {Name=AuthenticationConstants.RejectOperationName};
    }

 
}