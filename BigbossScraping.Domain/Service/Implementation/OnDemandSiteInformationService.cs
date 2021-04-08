using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BigbossScraping.Contracts.Interfaces;
using BigbossScraping.Contracts.Model;
using Microsoft.Extensions.Logging;

namespace BigbossScraping.Domain.Service.Implementation
{
    public class OnDemandSiteInformationService : IOnDemandSiteInformationService
    {
        private readonly ILogger<OnDemandSiteInformationService> _logger;
        private readonly IProgramCategoryScrapper _programCategoryScrapper;
        private readonly IProgrammeScrapper _programmeScrapper;
        private readonly IArticleScrapper _articleScrapper;

        public OnDemandSiteInformationService(
            ILogger<OnDemandSiteInformationService> logger,
            IProgramCategoryScrapper programCategoryScrapper,
            IProgrammeScrapper programmeScrapper,
            IArticleScrapper articleScrapper)
        {
            _logger = logger;
            _programCategoryScrapper = programCategoryScrapper;
            _programmeScrapper = programmeScrapper;
            _articleScrapper = articleScrapper;
        }

        public async Task<IDictionary<string, IList<Category>>> GetCategories(string siteUrl)
        {
            try
            {
                var tcs = new TaskCompletionSource<IDictionary<string, IList<Category>>>();
                _programCategoryScrapper.ParsedResponse += dictionary =>
                {
                    tcs.SetResult(dictionary);
                };
                await _programCategoryScrapper.StartPageScraper(siteUrl, true);

                await tcs.Task;

                return tcs.Task.Result;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
            }

            return await Task.FromResult(new Dictionary<string, IList<Category>>());
        }

        public Task<PagedProgramInformation> GetArticlesList(string categoryUrl)
        {
            try
            {
                var tcs = new TaskCompletionSource<PagedProgramInformation>();
                _articleScrapper.ParsedResponse += information => { tcs.SetResult(information); };
                _articleScrapper.StartPageScraper(categoryUrl, true);
                return tcs.Task;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
            }

            return Task.FromResult(new PagedProgramInformation());
        }

        public Task<ProgrammeDetails> GetProgramInformation(string articleUrl)
        {
            try
            {
                var tcs = new TaskCompletionSource<ProgrammeDetails>();
                _programmeScrapper.ParsedResponse += information => { tcs.SetResult(information); };
                _programmeScrapper.StartPageScraper(articleUrl, true);
                return tcs.Task;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
            }

            return Task.FromResult(new ProgrammeDetails());
        }
    }
}
