namespace InventoryManagementSystem.ApplicationCore.Entities
{
    public class ProductIndex : BaseEntity
    {
        public string Name { get; set; }
        
        [Nest.PropertyName("id")]
        public override string Id { get; set; }

    }
}