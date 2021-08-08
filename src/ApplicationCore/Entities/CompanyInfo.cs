namespace InventoryManagementSystem.ApplicationCore.Entities
{
    public class CompanyInfo : BaseEntity
    {
        public override string Id { get; set; } = "CPM_INFO";
        
        public string CompanyName { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string CompanyProfilePic { get; set; }
    }
}