using System;
using System.Linq;
using System.Threading.Tasks;
using BigbossScraping.Contracts.Interfaces;
using IronWebScraper;
using Microsoft.Extensions.Logging;

namespace BigbossScraping.IronScraper.Services
{
    class PagedProgramInformationScrapper : WebScraper, IPagedArticleScrapper
    {
        private readonly IPageScrapeScheduler _pageScrapeScheduler;
        private readonly ILogger<PagedProgramInformationScrapper> _logger;

        private string groupdId;
        private string pageUrl = string.Empty;
        private int pageStartNumber = 1;
        private int pageLastNumber = -1;

        public PagedProgramInformationScrapper(
            IPageScrapeScheduler pageScrapeScheduler,
            ILogger<PagedProgramInformationScrapper> logger)
        {
            _pageScrapeScheduler = pageScrapeScheduler;
            _logger = logger;
            LoggingLevel = LogLevel.Critical;
        }

        public override void Parse(Response response)
        {
            
        }

        private async Task ParseData(Response response)
        {
            string parentJob = null;
            try
            {
                if (response.WasSuccessful && response.CssExists(".page-numbers"))
                {
                    pageLastNumber =
                        int.Parse(response.Css(".page-numbers").Reverse().Skip(1).First()?.InnerTextClean ?? "-1");

                    for (int i = pageLastNumber; i >= pageStartNumber; i--)
                    {
                        _pageScrapeScheduler.ScheduleArticleJob(pageUrl + $"/page/{i}", parentJob, groupdId);
                        _logger.LogInformation($"Page {i} being scheduled {pageUrl}");
                        await Task.Delay(1000);
                    }

                    _logger.LogInformation($"Page being scheduled {pageUrl}");
                    await Task.Delay(1000);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
            }
        }

        public override void Init()
        {
            Request(pageUrl, async (response) => await ParseData(response));
        }

        public Task StartPageScraper(string pageUrl, bool onDemand = false, string groupId = null)
        {
            this.groupdId = groupId;
            this.pageUrl = pageUrl;

            return StartAsync();
        }
    }
}
