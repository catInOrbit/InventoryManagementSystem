using System;
using System.Threading;
using System.Threading.Tasks;
using Infrastructure.Identity.DbContexts;
using InventoryManagementSystem.ApplicationCore.Entities;
using InventoryManagementSystem.ApplicationCore.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services
{
    public class RedisWorkerService : BackgroundService
    {
        private readonly IRedisRepository _redisRepository;
        private readonly ILogger<RedisWorkerService> _logger;
        public RedisWorkerService(IRedisRepository redisRepository, ILogger<RedisWorkerService> logger)
        {
            _redisRepository = redisRepository;
            _logger = logger;
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var data = await _redisRepository.GetNotificationAll("Notifications");
                _logger.LogInformation("Worker for redis running at: {0}", DateTimeOffset.Now);
                if (data != null)
                {
                    await using (var dbContext = new IdentityAndProductDbContext(new DbContextOptions<IdentityAndProductDbContext>()))
                    {
                        await dbContext.AddRangeAsync(data, stoppingToken);
                        dbContext.Entry(data).State = EntityState.Added;
                        await dbContext.SaveChangesAsync(stoppingToken);
                    }
                }
                await Task.Delay(3600000, stoppingToken);
            }
        }
    }
}