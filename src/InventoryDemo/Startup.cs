using InventoryDemo.BackgroundServices.ScheduledServices;
using InventoryDemo.Context;
using InventoryDemo.Events;
using InventoryDemo.Extensions;
using InventoryDemo.Services.Cache;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;
using System.IO.Compression;
using System.Threading.Tasks;

namespace InventoryDemo
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public static readonly ILoggerFactory loggerFactory = LoggerFactory.Create(builder => { builder.AddConsole(); });

        public Startup(IConfiguration configuration) => Configuration = configuration;

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContextPool<InventoryContext>(options =>
                options.UseLoggerFactory(loggerFactory)
                    .EnableSensitiveDataLogging()
                    .UseSqlServer(Configuration.GetConnectionString("InventoryDatabase")));

            services.AddSingleton<IConnectionMultiplexer>(c =>
                ConnectionMultiplexer.Connect(Configuration.GetConnectionString("RedisConnection")));
            services.AddSingleton<ICacheService, RedisCacheService>();

            services.AddCronJob<OrderJobService>(c =>
            {
                c.TimeZoneInfo = TimeZoneInfo.Local;
                c.CronExpression = @"*/1 * * * *";
            });

            services.ConfigureCors();
            services.ConfigureBackgroundServices();
            services.ConfigureServices();
            services.ConfigureRepositories();
            services.ConfigureFactories();
            services.ConfigureContexts();
            services.ConfigureSwagger();
            services.ConfigureAuthentication(Configuration);

            services.Configure<BrotliCompressionProviderOptions>(options => options.Level = CompressionLevel.Fastest);
            services.Configure<GzipCompressionProviderOptions>(options => options.Level = CompressionLevel.Fastest);
            services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
                options.Providers.Add<BrotliCompressionProvider>();
                options.Providers.Add<GzipCompressionProvider>();
            });

            services.AddRabbitMQ();
            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, InventoryContext context, IBus bus)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            context.Database.EnsureCreated();
            app.UseResponseCompression();
            app.UseCors("CorsPolicy");

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "InventoryDemo v1"));

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseProblemDetailsExceptionHandler();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers().RequireAuthorization();
            });

            var updateOrder = new Task(async () =>
            {
                await bus.Publish(new OrderEvent { RequestedAt = DateTime.Now, ForceUpdate = false });
            });
            updateOrder.RunSynchronously();
        }
    }
}
