using System.Threading.Tasks;
using InventoryManagementSystem.ApplicationCore.Entities;
using Nest;

namespace InventoryManagementSystem.ApplicationCore.Interfaces
{
    public interface IElasticAsyncRepository<T> where T : BaseEntity
    {
        Task ElasticSaveSingleAsync(bool isSavingNew, T types, string index);
        Task ElasticSaveManyAsync(T[] types);
        Task ElasticSaveBulkAsync(T[] types, string index);
        Task ElasticDeleteSingleAsync(T type, string index);
    }
}