using InventoryDemo.Context;
using InventoryDemo.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryDemo
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public static readonly ILoggerFactory loggerFactory = LoggerFactory.Create(builder => { builder.AddConsole(); });

        public Startup(IConfiguration configuration) => Configuration = configuration;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContextPool<InventoryContext>(options =>
                options.UseLoggerFactory(loggerFactory)
                    .EnableSensitiveDataLogging()
                    .UseSqlServer(Configuration.GetConnectionString("InventoryDatabase")));

            services.ConfigureBackgroundServices();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "InventoryDemo", Version = "v1" });
            });

            services.Configure<BrotliCompressionProviderOptions>(options => options.Level = CompressionLevel.Fastest);
            services.Configure<GzipCompressionProviderOptions>(options => options.Level = CompressionLevel.Fastest);
            services.AddResponseCompression(options =>
            {
                options.Providers.Add<BrotliCompressionProvider>();
                options.Providers.Add<GzipCompressionProvider>();
            });

            services.AddControllers().AddJsonOptions(options => options.JsonSerializerOptions.IgnoreNullValues = true);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, InventoryContext context)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            context.Database.EnsureCreated();
            app.UseResponseCompression();

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "InventoryDemo v1"));

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
