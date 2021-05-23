using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryManagementSystem.PublicApi.ManagerEndpoints
{
    public class RoleDeleteResponse : BaseResponse
    {
        public RoleDeleteResponse(Guid correlationId) : base(correlationId)
        {}

        public RoleDeleteResponse()
        {

        }

        public bool Result { get; set; }
        public string Verbose { get; set; }
        public string Role { get; set; }
    }
}
