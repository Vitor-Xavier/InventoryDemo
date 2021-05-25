using InventoryDemo.Services.Cache;
using InventoryDemo.Services.Orders;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace InventoryDemo.BackgroundServices.ScheduledServices
{
    public class OrderJobService : CronJobService
    {
        private readonly IServiceProvider _serviceProvider;

        private readonly ILogger<OrderJobService> _logger;

        public OrderJobService(IScheduleConfig<OrderJobService> config,
                               ILogger<OrderJobService> logger,
                               IServiceProvider serviceProvider) : base(config.CronExpression, config.TimeZoneInfo)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Serviço de execução de tarefas agendadas está rodando.");
            return base.StartAsync(cancellationToken);
        }

        public override async Task DoWork(CancellationToken cancellationToken)
        {
            using var scopeService = _serviceProvider.CreateScope();
            var cacheService = scopeService.ServiceProvider.GetService<ICacheService>();
            
            var latestUpdate = await cacheService.GetCacheValue<DateTime>("latest.update:orders");
            if (latestUpdate <= DateTime.Now.AddHours(-1))
            {
                _logger.LogInformation($"{DateTime.Now:dd/MM/yyyy HH:mm:ss} Atualização de pedidos iniciada.");
                try
                {
                    var orderService = scopeService.ServiceProvider.GetService<IOrderService>();
                    await orderService.UpdateCacheOrders();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro na atualização de pedidos");
                }
                finally
                {
                    await cacheService.SetCacheValue("latest.update:orders", DateTime.Now);
                }
                _logger.LogInformation($"Atualização de pedidos finalizada.");
            }
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Serviço de execução de tarefas agendadas está sendo interrompido.");
            return base.StopAsync(cancellationToken);
        }
    }
}
