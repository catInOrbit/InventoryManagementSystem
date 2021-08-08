using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Nest;

namespace InventoryManagementSystem.ApplicationCore.Entities
{
    // This can easily be modified to be BaseEntity<T> and public T Id to support different key types.
    // Using non-generic integer types for simplicity and to ease caching logic
    public abstract class BaseEntity
    {
        [JsonIgnore]
        [PropertyName("id")]
        [MaxLength(50)]
        public virtual string Id { get; set; }
    }
}
