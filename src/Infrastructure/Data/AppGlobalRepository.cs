using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.Specification;
using Infrastructure.Identity;
using Infrastructure.Identity.DbContexts;
using InventoryManagementSystem.ApplicationCore.Entities;
using InventoryManagementSystem.ApplicationCore.Entities.Orders;
using InventoryManagementSystem.ApplicationCore.Entities.Products;
using InventoryManagementSystem.ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Nest;

namespace Infrastructure.Data
{
    public class AppGlobalRepository<T> : IAsyncRepository<T> where T : BaseEntity
    {
        private readonly IdentityAndProductDbContext _identityAndProductDbContext;
        private readonly IElasticClient _elasticClient;

        private List<T> _elasticCacheProduct = new List<T>();


        public AppGlobalRepository(IdentityAndProductDbContext identityAndProductDbContext, IElasticClient elasticClient)
        {
            _identityAndProductDbContext = identityAndProductDbContext;
            _elasticClient = elasticClient;
        }

        public async Task<T> GetByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            var keyValues = new object[] { id };
            return await _identityAndProductDbContext.Set<T>().FindAsync(keyValues, cancellationToken);
        }

        public async Task<IReadOnlyList<ProductIndex>> GetProductForELIndexAsync(CancellationToken cancellationToken = default)
        {
            var products= await _identityAndProductDbContext.Set<Product>().Select(p=> new {p.Id, p.Name}).ToListAsync(cancellationToken);
            var prodcuctIndices = new List<ProductIndex>();
            foreach (var product in products)
            {
                ProductIndex productIndex = new ProductIndex
                {
                    Id = product.Id,
                    Name = product.Name
                };
                prodcuctIndices.Add(productIndex);
            }

            return prodcuctIndices;
        }

        public PriceQuoteOrder GetPriceQuoteByNumber(string priceQuoteNumber, CancellationToken cancellationToken = default)
        {
            return _identityAndProductDbContext.PriceQuote.Where(pq => pq.PriceQuoteOrderNumber == priceQuoteNumber).
                SingleOrDefault(pq => pq.PriceQuoteOrderNumber == priceQuoteNumber);
        }

        public PurchaseOrder GetPurchaseOrderByNumber(string purchaseOrderNumber, CancellationToken cancellationToken = default)
        {
            return _identityAndProductDbContext.PurchaseOrder.Where(po => po.PurchaseOrderNumber == purchaseOrderNumber).
                SingleOrDefault(po => po.PurchaseOrderNumber == purchaseOrderNumber);   
        }

        public async Task<IReadOnlyList<T>> ListAllAsync(CancellationToken cancellationToken = default)
        {
            return await _identityAndProductDbContext.Set<T>().ToListAsync(cancellationToken);
        }
        
        public async Task<IEnumerable<Product>> ListAllProductAsync(CancellationToken cancellationToken = default)
        {
            var query =  await _identityAndProductDbContext.Set<Product>()
                .Join(
                    _identityAndProductDbContext.ProductVariant,
                    product => product.Id,
                    variant => variant.ProductId,
                    (entryPoint, entry) => new {entryPoint, entry}
                ).ToListAsync(cancellationToken);
            return (IEnumerable<Product>) query;
        }

        public Task<IReadOnlyList<T>> ListAsync(ISpecification<T> spec, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

        public async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            await _identityAndProductDbContext.Set<T>().AddAsync(entity);
            await _identityAndProductDbContext.SaveChangesAsync(cancellationToken);

            return entity;
        }

        public async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
        {
            _identityAndProductDbContext.Entry(entity).State = EntityState.Modified;
            await _identityAndProductDbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
        {
            _identityAndProductDbContext.Set<T>().Remove(entity);
            await _identityAndProductDbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task DeletePurchaseOrderAsync(PurchaseOrder entity, CancellationToken cancellationToken = default)
        {
            var po = _identityAndProductDbContext.PurchaseOrder.Where(po => po.Id == entity.Id);
            var poItems = _identityAndProductDbContext.OrderItem.Where(poItem => poItem.OrderNumber == entity.Id);

            _identityAndProductDbContext.RemoveRange(poItems);
            _identityAndProductDbContext.Remove(po);
            await _identityAndProductDbContext.SaveChangesAsync(cancellationToken);
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
        
        public async Task ElasticSaveSingleAsync(T type)
        {
            if (_elasticCacheProduct.Any(p => p.Id == type.Id))
            {
                await _elasticClient.UpdateAsync<T>(type, u => u.Doc(type));
            }
            else
            {
                _elasticCacheProduct.Add(type);
                await _elasticClient.IndexDocumentAsync<T>(type);
            }
        }

        public async Task ElasticSaveManyAsync(T[] types)
        {
            _elasticCacheProduct.AddRange(types);
            var result = await _elasticClient.IndexManyAsync(types);
            if (result.Errors)
            {
                // the response can be inspected for errors
                foreach (var itemWithError in result.ItemsWithErrors)
                {
                    throw new Exception();
                }
            }
        }

        public async Task ElasticSaveBulkAsync(T[] types)
        {
            _elasticCacheProduct.AddRange(types);
            var result = await _elasticClient.BulkAsync(b => b.Index("products").IndexMany(types));
            if (result.Errors)
            {
                // the response can be inspected for errors
                foreach (var itemWithError in result.ItemsWithErrors)
                {
                    throw new Exception();
                }
            }
        }
    }
}