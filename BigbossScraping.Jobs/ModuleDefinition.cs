using System;
using BigbossScraping.Contracts.Interfaces;
using BigbossScraping.Jobs.Services;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.Extensions.DependencyInjection;

namespace BigbossScraping.Jobs
{
    public class ModuleDefinition
    {
        public static void Load(IServiceCollection services, string connectionString = null)
        {
            //services.AddHangfire(configuration =>
            //{
            //    configuration.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
            //        .UseSimpleAssemblyNameTypeSerializer()
            //        .UseRecommendedSerializerSettings()
            //        .UseSqlServerStorage(connectionString, new SqlServerStorageOptions
            //        {
            //            SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
            //            QueuePollInterval = TimeSpan.FromSeconds(30),
            //            DisableGlobalLocks = true,
            //            UseRecommendedIsolationLevel = true,
            //            UsePageLocksOnDequeue = true,
            //            CommandBatchMaxTimeout = TimeSpan.FromMinutes(5)
            //        });
            //});

            //services.AddHangfireServer(options => { options.WorkerCount = Environment.ProcessorCount * 6; });
            //GlobalJobFilters.Filters.Add(new AutomaticRetryAttribute { Attempts = 5 });

            services.AddScoped<IPageScrapeScheduler, PageScrapeScheduler>();
        }
    }
}
