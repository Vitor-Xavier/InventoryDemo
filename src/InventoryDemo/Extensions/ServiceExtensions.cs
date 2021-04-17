using InventoryDemo.BackgroundServices.QueuedServices;
using InventoryDemo.BackgroundServices.ScheduledServices;
using InventoryDemo.Crosscutting;
using InventoryDemo.Repositories.Products;
using InventoryDemo.Repositories.Suppliers;
using InventoryDemo.Services.Products;
using InventoryDemo.Services.Suppliers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;

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

        public static void ConfigureAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var appSettingsSection = configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);

            var appSettings = appSettingsSection.Get<AppSettings>();
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                };
            });
        }
    }
}
