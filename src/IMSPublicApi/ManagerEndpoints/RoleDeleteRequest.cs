using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryManagementSystem.PublicApi.ManagerEndpoints
{
    public class RoleDeleteRequest : BaseRequest
    {
        public string Role { get; set; }
    }
}
