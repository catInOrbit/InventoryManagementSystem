using System;
using System.Collections.Generic;
using InventoryManagementSystem.ApplicationCore.Entities;
using Microsoft.AspNetCore.Identity;

namespace InventoryManagementSystem.PublicApi.ManagerEndpoints
{
    public class GetAllRoleResponse : BaseResponse
    {
        public GetAllRoleResponse(Guid correlationId) : base(correlationId)
        {
                
        }

        public GetAllRoleResponse()
        {
            
        }
        // public List<IdentityRole> Roles { get; set; }
        
        public PagingOption<IdentityRole> Paging { get; set; }
    }

    public class GetAllRoleRequest : BaseRequest
    {
        public int CurrentPage { get; set; }
        public int SizePerPage { get; set; }
    }


    public class GetSpecificRoleRequest
    {
        public string RoleId { get; set; }
    }
    
    public class GetSpecificRoleResponse
    {
        public IdentityRole Role { get; set; }
        public Dictionary<string, List<string>> PagePermissions { get; set; } = new Dictionary<string, List<string>>();
        
        
        // private List<Dictionary<string, List<string>>> PagePermissions = new List<Dictionary<string, List<string>>>();
        // public List<PagePermissions> PagePermissions { get; set; } = new List<PagePermissions>();
    }

    public class PagePermissions
    {
        public string PageName { get; set; }
        public List<String> Permissions { get; set; }
    }
    
    public class PermissionInfo
    {

    }
}