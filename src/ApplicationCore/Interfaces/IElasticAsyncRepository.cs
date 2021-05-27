using System.Collections.Generic;
using System.Threading.Tasks;
using InventoryManagementSystem.ApplicationCore.Entities;

namespace InventoryManagementSystem.ApplicationCore.Interfaces
{
    public interface IElasticAsyncRepository<T> where T : BaseEntity
    {
        Task<IEnumerable<T>> GetAll(int count, int skip = 0);
        Task<T> GetByID(int id);
        Task<T> GetByCategory(string category);
        Task<T> GetByBrand(string category);
    }
}