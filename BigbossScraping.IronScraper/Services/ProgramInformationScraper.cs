using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BigbossScraping.Contracts.Interfaces;
using BigbossScraping.Contracts.Model;
using BigbossScraping.IronScraper.Extensions;
using IronWebScraper;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace BigbossScraping.IronScraper.Services
{
    class ProgramInformationScraper : WebScraper, IArticleScrapper
    {
        private readonly IPageScrapeScheduler _pageScrapeScheduler;
        private readonly ILogger<ProgramInformationScraper> _logger;
        private string _pageUrl;
        private string _groupId;
        private bool _onDemand;
        private readonly IList<ProgramInformation> _articleResponse;

        public ProgramInformationScraper(IPageScrapeScheduler pageScrapeScheduler, ILogger<ProgramInformationScraper> logger)
        {
            _pageScrapeScheduler = pageScrapeScheduler;
            _logger = logger;
            LoggingLevel = LogLevel.Critical;
            _articleResponse = new List<ProgramInformation>();
        }

        public override void Parse(Response response)
        {

        }

        public override void Init()
        {
            Request(_pageUrl, async (res) => await ParseData(res));
        }

        private async Task ParseData(Response response)
        {
            await LocalAsync();

            async Task LocalAsync()
            {
                string parentJob = null;

                foreach (var res in response.Document.GetArticleNodes())
                {
                    var article = res.GetArticleFromNode(_groupId);
                    _logger.LogInformation(
                        $"Article being scraped {article.Id} - {article.Title} - {article.CategoryId} - {article.Url}");
                    _articleResponse.Add(article);
                }

                if (!_onDemand)
                {
                    _pageScrapeScheduler.RaisePageScrapeCompleted(JsonConvert.SerializeObject(_articleResponse),
                        ResponseType.Article);

                    foreach (var article in _articleResponse)
                    {
                        await Task.Delay(TimeSpan.FromMilliseconds(500));
                        _pageScrapeScheduler.ScheduleProgramJob(article?.Url, parentJob, article?.Id.ToString());
                        _logger.LogInformation($"Article information scheduled to scrape video details {article?.Url}");
                    }
                }
                else
                {
                    GeneratePagination(response);
                }
            }
        }

        private void GeneratePagination(Response response)
        {
            var paginationDetails = new List<Pagination>();
            foreach (var htmlNode in response.Css(".page-numbers"))
            {
                var pagination = new Pagination();
                if (htmlNode.Attributes.First().Value.Contains("current"))
                {
                    pagination.IsCurrent = true;
                    pagination.PageUrl = _pageUrl;
                    pagination.PageNumber = htmlNode.GetCleanTextFromNode();
                }
                else
                {
                    pagination.PageUrl = htmlNode.GetHyperlinkReferenceFromAnchor();
                    pagination.PageNumber = htmlNode.GetCleanTextFromNode();
                }

                paginationDetails.Add(pagination);
            }

            ParsedResponse(new PagedProgramInformation
                {ProgramInformations = _articleResponse, PaginationDetail = paginationDetails});
        }

        public Task StartPageScraper(string pageUrl, bool onDemand = false, string groupId = null)
        {
            _pageUrl = pageUrl;
            _groupId = groupId;
            _onDemand = onDemand;

            return StartAsync();
        }

        public Action<PagedProgramInformation> ParsedResponse { get; set; }
    }
}