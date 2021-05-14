namespace ApplicationCore.Entities
{
    public class Product : BaseEntity
    {
        public string Name { get; private set; }
        public string SerialNunmber { get; private set; }
        public string Sku { get; private set; }
    }
}