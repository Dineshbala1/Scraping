using BigbossScraping.Contracts.Interfaces;
using BigbossScraping.IronScraper.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BigbossScraping.IronScraper
{
    public class ModuleDefinition
    {
        public static void Load(IServiceCollection services)
        {
            services.AddScoped<IProgramCategoryScrapper, ProgramCategoryScrapper>();
            services.AddScoped<IPagedArticleScrapper, PagedProgramInformationScrapper>();
            services.AddScoped<IArticleScrapper, ProgramInformationScraper>();
            services.AddScoped<IProgrammeScrapper, ProgramDetailsScraper>();
            services.AddScoped<IHomePageScrapper, HomePageScrapper>();
        }
    }
}
