using InventoryDemo.BackgroundServices.QueuedServices;
using InventoryDemo.BackgroundServices.ScheduledServices;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace InventoryDemo.Extensions
{
    public static class ServiceExtensions
    {
        public static void ConfigureBackgroundServices(this IServiceCollection services)
        {
            services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();

            services.AddQueuedService<QueuedHostedService>(c =>
            {
                c.ConcurrentTasks = Environment.ProcessorCount;
            });
        }

        public static IServiceCollection AddCronJob<T>(this IServiceCollection services, Action<IScheduleConfig<T>> options) where T : CronJobService
        {
            if (options is null)
                throw new ArgumentNullException(nameof(options), @"Configurações do serviço agendado estão ausentes.");

            var config = new ScheduleConfig<T>();
            options.Invoke(config);
            if (string.IsNullOrWhiteSpace(config.CronExpression))
                throw new ArgumentNullException(nameof(ScheduleConfig<T>.CronExpression), @"Expressão Cron não permitida.");

            services.AddSingleton<IScheduleConfig<T>>(config);
            services.AddHostedService<T>();

            return services;
        }

        public static IServiceCollection AddQueuedService<T>(this IServiceCollection services, Action<IQueuedConfig<T>> options) where T : QueuedHostedService
        {
            if (options is null)
                throw new ArgumentNullException(nameof(options), @"Configurações do serviço agendado estão ausentes.");

            var config = new QueuedConfig<T>();
            options.Invoke(config);

            services.AddSingleton<IQueuedConfig<T>>(config);
            services.AddHostedService<T>();

            return services;
        }
    }
}
