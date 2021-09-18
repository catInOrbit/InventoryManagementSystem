using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Infrastructure.Identity.DbContexts;
using InventoryManagementSystem.ApplicationCore;
using InventoryManagementSystem.ApplicationCore.Constants;
using InventoryManagementSystem.ApplicationCore.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
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
                await _redisRepository.ClearNotification("Notifications");
                _logger.LogInformation("Worker for redis running at: {0}", DateTimeOffset.Now);
                if (data != null)
                {
                    DbContextOptionsBuilder<IdentityAndProductDbContext> options =
                        new DbContextOptionsBuilder<IdentityAndProductDbContext>(
                        );
                    options.UseSqlServer(_configuration.GetConnectionString(ConnectionPropertiesConstant.MAIN_CONNECTION_STRING));
                    using(var dbContext = new IdentityAndProductDbContext(options.Options))
                    {
                        var activated = from p in dbContext.AdminControlOptions select p;
                        var adminControl = activated as AdminControlOptions;
                        if (adminControl.IsActivatingRedisNotiWorker == true)
                        {
                            await dbContext.AddRangeAsync(data, stoppingToken);
                            await dbContext.SaveChangesAsync(stoppingToken);    
                        }
                    }
                }
                await Task.Delay(3600000, stoppingToken);
            }
        }
    }
}