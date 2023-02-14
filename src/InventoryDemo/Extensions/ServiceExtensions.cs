using InventoryDemo.BackgroundServices.QueuedServices;
using InventoryDemo.BackgroundServices.ScheduledServices;
using InventoryDemo.Consumers;
using InventoryDemo.Crosscutting;
using InventoryDemo.Infrastructure.Hubs;
using InventoryDemo.Infrastructure.Repositories.Notifications;
using InventoryDemo.Infrastructure.Repositories.OrderExports;
using InventoryDemo.Infrastructure.Repositories.OrderImports;
using InventoryDemo.Infrastructure.Repositories.Orders;
using InventoryDemo.Infrastructure.Repositories.Products;
using InventoryDemo.Infrastructure.Repositories.Suppliers;
using InventoryDemo.Infrastructure.Repositories.Users;
using InventoryDemo.Infrastructure.Repositories.UsersNotifications;
using InventoryDemo.Providers;
using InventoryDemo.Services.CancellationHashs.OrderExports;
using InventoryDemo.Services.CancellationHashs.OrderImports;
using InventoryDemo.Services.Contexts;
using InventoryDemo.Services.Factories;
using InventoryDemo.Services.Notifications;
using InventoryDemo.Services.OrderExports;
using InventoryDemo.Services.Orders;
using InventoryDemo.Services.Products;
using InventoryDemo.Services.Suppliers;
using InventoryDemo.Services.Users;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace InventoryDemo.Extensions
{
    public static class ServiceExtensions
    {
        public static void ConfigureServices(this IServiceCollection services)
        {
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ISupplierService, SupplierService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IOrderExportService, OrderExportService>();
            services.AddScoped<IOrderImportService, OrderImportService>();
            services.AddScoped<INotificationService, NotificationService>();

            services.AddSingleton<IOrderExportCancellationHash, OrderExportCancellationHash>();
            services.AddSingleton<IOrderImportCancellationHash, OrderImportCancellationHash>();
        }

        public static void ConfigureRepositories(this IServiceCollection services)
        {
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<ISupplierRepository, SupplierRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IOrderExportRepository, OrderExportRepository>();
            services.AddScoped<IOrderImportRepository, OrderImportRepository>();
            services.AddScoped<INotificationRepository, NotificationRepository>();
            services.AddScoped<IUserNotificationRepository, UserNotificationRepository>();
        }

        public static void ConfigureBackgroundServices(this IServiceCollection services)
        {
            services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();

            services.AddQueuedService<QueuedHostedService>(c =>
            {
                c.ConcurrentTasks = Environment.ProcessorCount;
            });
        }

        public static void ConfigureProviders(this IServiceCollection services)
        {
            services.AddSingleton<IUserIdProvider, UsernameBasedUserIdProvider>();
        }

        public static void ConfigureContexts(this IServiceCollection services)
        {
            services.AddScoped<IOrderFormatContext, OrderFormatContext>();
        }

        public static void ConfigureFactories(this IServiceCollection services)
        {
            services.AddScoped<IOrderFormatFactory, OrderFormatFactory>();
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
                x.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];

                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments(NotificationHub.ROUTE))
                            context.Token = accessToken;

                        return Task.CompletedTask;
                    }
                };
            });
        }

        public static void AddRabbitMQ(this IServiceCollection services)
        {
            services.AddMassTransit(config =>
            {
                config.AddConsumer<OrderConsumer>(consumerConfig =>
                {
                    consumerConfig.UseConcurrencyLimit(1);

                    consumerConfig.UseRetry(retryConfig =>
                    {
                        retryConfig.Interval(3, TimeSpan.FromMilliseconds(5000));
                    });
                });
                
                config.AddConsumer<OrderExportConsumer>(consumerConfig =>
                {
                    consumerConfig.UseConcurrencyLimit(3);

                    consumerConfig.UseRetry(retryConfig =>
                    {
                        retryConfig.Interval(3, TimeSpan.FromMilliseconds(5000));
                    });
                });

                config.AddConsumer<OrderImportConsumer>(consumerConfig =>
                {
                    consumerConfig.UseConcurrencyLimit(3);

                    consumerConfig.UseRetry(retryConfig =>
                    {
                        retryConfig.Interval(3, TimeSpan.FromMilliseconds(5000));
                    });
                });

                config.UsingRabbitMq((context, configure) =>
                {
                    configure.Host("rabbitmq://localhost", h =>
                    {
                        h.Username("guest");
                        h.Password("guest");
                    });

                    configure.ConfigureEndpoints(context);
                });
            });
        }

        internal static void ConfigureSwagger(this IServiceCollection services) =>
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Inventory",
                    Version = "v1",
                    Contact = new OpenApiContact
                    {
                        Name = "Vitor Xavier de Souza",
                        Email = "vitorvxs@live.com",
                        Url = new Uri("https://github.com/Vitor-Xavier")
                    },
                    License = new OpenApiLicense
                    {
                        Name = "MIT License",
                        Url = new Uri("https://opensource.org/licenses/MIT")
                    }
                });

                c.AddSecurityDefinition("bearerAuth", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Description = "JWT Authorization header using the Bearer scheme.",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT"

                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "bearerAuth" }
                        },
                        Array.Empty<string>()
                    }
                });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

        public static void ConfigureCors(this IServiceCollection services) =>
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder => builder
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .SetIsOriginAllowed(_ => true)
                    .AllowCredentials());
            });
    }
}
