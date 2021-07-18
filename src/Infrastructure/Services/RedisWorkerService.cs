using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Infrastructure.Identity.DbContexts;
using InventoryManagementSystem.ApplicationCore.Constants;
using InventoryManagementSystem.ApplicationCore.Entities;
using InventoryManagementSystem.ApplicationCore.Entities.Notifications;
using InventoryManagementSystem.ApplicationCore.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services
{
    public class RedisWorkerService : BackgroundService
    {
        private readonly IRedisRepository _redisRepository;
        private readonly ILogger<RedisWorkerService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private  readonly IConfiguration _configuration;
        public RedisWorkerService(IRedisRepository redisRepository, ILogger<RedisWorkerService> logger, IServiceProvider serviceProvider, IConfiguration configuration)
        {
            _redisRepository = redisRepository;
            _logger = logger;
            _serviceProvider = serviceProvider;
            _configuration = configuration;
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var data = await _redisRepository.GetNotificationAll("Notifications");
                _logger.LogInformation("Worker for redis running at: {0}", DateTimeOffset.Now);
                if (data != null)
                {
                    DbContextOptionsBuilder<IdentityAndProductDbContext> options =
                        new DbContextOptionsBuilder<IdentityAndProductDbContext>(
                        );
                    options.UseSqlServer(_configuration.GetConnectionString(ConnectionPropertiesConstant.MAIN_CONNECTION_STRING));
                    using(var dbContext = new IdentityAndProductDbContext(options.Options))
                    {
                        await dbContext.AddRangeAsync(data, stoppingToken);
                        await dbContext.SaveChangesAsync(stoppingToken);
                    }
                }
        
                
                await _redisRepository.ClearNotification("Notifications");
                
                await Task.Delay(3600000, stoppingToken);
            }
        }
    }
}