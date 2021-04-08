using System;
using System.Reflection;
using BigbossScraping.DataAccess;
using BigbossScraping.Domain;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace BigbossScraping.Extensions
{
    static class ServiceCollectionExtensions
    {
        private const string SqlConnection = nameof(SqlConnection);
        private const string HangFireConnection = nameof(HangFireConnection);

        public static IServiceCollection AddSerilog(this IServiceCollection services, IConfiguration configuration)
        {
            Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(configuration)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();

            services.AddLogging(lb => lb.AddSerilog(Log.Logger, dispose: true));
            return services;
        }

        public static IServiceCollection AddKestrelConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<KestrelServerOptions>(
                configuration.GetSection("Kestrel"));
            return services;
        }

        public static IServiceCollection AddSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.IgnoreObsoleteActions();
                options.CustomSchemaIds(t => t.FullName);
                options.IncludeXmlComments(
                    $"{AppDomain.CurrentDomain.BaseDirectory}\\{Assembly.GetExecutingAssembly().GetName().Name}.XML");
            });

            return services;
        }

        public static IServiceCollection AddSqlContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ScrapingDbContext>(op =>
                op.UseSqlServer(configuration.GetConnectionString(SqlConnection)));
            services.AddScoped<IScrapingContext, ScrapingDbContext>();
            return services;
        }

        public static IServiceCollection LoadModules(this IServiceCollection services, IConfiguration configuration)
        {
            IronScraper.ModuleDefinition.Load(services);
            AgilityScraper.ModuleDefinition.Load(services);
            Domain.ModuleDefinition.Load(services);
            Jobs.ModuleDefinition.Load(services, configuration.GetConnectionString(HangFireConnection));

            return services;
        }
    }
}
