using BigbossScraping.Contracts.Interfaces;
using BigbossScraping.Domain.Service;
using BigbossScraping.Domain.Service.Implementation;
using Microsoft.Extensions.DependencyInjection;

namespace BigbossScraping.Domain
{
    public class ModuleDefinition
    {
        public static void Load(IServiceCollection services, string connectionString = null)
        {
            services.AddScoped<ISiteService, SiteService>();
            services.AddScoped<IScheduledSiteInformationService, ScrapedSiteService>();
            services.AddScoped<IOnDemandSiteInformationService, OnDemandSiteInformationService>();
        }
    }
}
