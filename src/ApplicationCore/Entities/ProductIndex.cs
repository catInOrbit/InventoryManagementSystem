namespace InventoryManagementSystem.ApplicationCore.Entities
{
    public class ProductIndex : BaseEntity
    {
        [Nest.PropertyName("name")]
        public string Name { get; set; }
        
        [Nest.PropertyName("id")]
        public override string Id { get; set; }

    }
}