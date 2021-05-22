using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.Specification;
using Infrastructure.Identity;
using InventoryManagementSystem.ApplicationCore.Entities;
using InventoryManagementSystem.ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class UserRepository<T> : IAsyncRepository<T> where T : BaseEntity
    {
        private readonly ApplicationIdentityDbContext _applicationIdentityDbContext;

        public UserRepository(ApplicationIdentityDbContext applicationIdentityDbContext)
        {
            _applicationIdentityDbContext = applicationIdentityDbContext;
        }

        public async Task<T> GetByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            var keyValues = new object[] { id };
            return await _applicationIdentityDbContext.Set<T>().FindAsync(keyValues, cancellationToken);
        }

        public async Task<IReadOnlyList<T>> ListAllAsync(CancellationToken cancellationToken = default)
        {
            return await _applicationIdentityDbContext.Set<T>().ToListAsync(cancellationToken);
        }

        public Task<IReadOnlyList<T>> ListAsync(ISpecification<T> spec, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

        public async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            await _applicationIdentityDbContext.Set<T>().AddAsync(entity);
            await _applicationIdentityDbContext.SaveChangesAsync(cancellationToken);

            return entity;
        }

        public Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

        public Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

        public Task<int> CountAsync(ISpecification<T> spec, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

        public Task<T> FirstAsync(ISpecification<T> spec, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

        public Task<T> FirstOrDefaultAsync(ISpecification<T> spec, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }
    }
}