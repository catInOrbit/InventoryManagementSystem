using System;

namespace Microsoft.eShopWeb.ApplicationCore.Entities
{
    public class IMSUser
    {
        public string OwnerID { get; set; }
        public string Fullname { get; set; }
        public DateTime ShiftStart { get; set; }
        public DateTime ShiftEnd { get; set; }
    }
}