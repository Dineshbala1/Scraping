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
    class HomePageScrapper : WebScraper, IHomePageScrapper
    {
        private readonly IScheduledSiteInformationService _scheduledSiteInformationService;
        private readonly IPageScrapeScheduler _pageScrapeScheduler;
        private readonly ILogger<HomePageScrapper> _logger;
        private string _pageUrl = string.Empty;
        private readonly IList<ProgramInformation> _articleResponse;

        public HomePageScrapper(IScheduledSiteInformationService scheduledSiteInformationService , IPageScrapeScheduler pageScrapeScheduler, ILogger<HomePageScrapper> logger)
        {
            _scheduledSiteInformationService = scheduledSiteInformationService;
            _pageScrapeScheduler = pageScrapeScheduler;
            _logger = logger;
            _articleResponse = new List<ProgramInformation>();
        }

        public Task StartPageScraper(string pageUrl, bool pagination = false, string groupId = null)
        {
            // Do Nothing
            return Task.CompletedTask;
        }


        public async Task ParseData(Response response)
        {
            try
            {
                var categoryTask = await _scheduledSiteInformationService.GetCategories();
                foreach (var articleNode in response.Document.GetArticleNodes())
                {
                    var attributeTexts = articleNode.GetAttribute("class")
                        .Split(new String[] { "category" }, StringSplitOptions.RemoveEmptyEntries);
                    var categoryTags = attributeTexts.Select(x => x.Replace('-', ' '));
                    var article = articleNode?.GetArticleFromNode(findParentId: title =>
                    {
                        if (MatchingCategoryId(categoryTask, title, categoryTags, out var s1)) return s1;

                        return Guid.Empty;
                    });

                    if (article.CategoryId == Guid.Empty)
                    {
                        _logger.LogError(new Exception($"Invalid Category Id for {article.Title} - {article.Url}"),
                            article.Url);
                        continue;
                    }

                    _articleResponse.Add(article);
                }

                _pageScrapeScheduler.RaisePageScrapeCompleted(JsonConvert.SerializeObject(_articleResponse),
                    ResponseType.Article);

                await Task.Delay(TimeSpan.FromMinutes(1));

                foreach (var article in _articleResponse)
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(500));
                    _pageScrapeScheduler.ScheduleProgramJob(article?.Url);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
            }
        }

        public override void Parse(Response response)
        {

        }

        private static bool MatchingCategoryId(IList<Category> categoryTask, string title,
            IEnumerable<string> categoryTags, out Guid s1)
        {
            foreach (var programCategory in categoryTask)
            {
                if (title.Contains(programCategory.Title.TrimEnd('s')))
                {
                    {
                        s1 = programCategory.Id;
                        return true;
                    }
                }

                foreach (var categoryTag in categoryTags)
                {
                    if (categoryTag.Contains(programCategory.Title.TrimEnd('s')) ||
                        string.Equals(categoryTag.TrimStart(' '), programCategory.Title,
                            StringComparison.OrdinalIgnoreCase) ||
                        categoryTag.TrimStart(' ').Split(new[] { " " }, StringSplitOptions.None).All(x =>
                          {
                              if (string.Equals(x, "tv", StringComparison.OrdinalIgnoreCase))
                              {
                                  return true;
                              }

                              return programCategory.Title.ToLower().Contains(x);
                          }))
                    {
                        {
                            s1 = programCategory.Id;
                            return true;
                        }
                    }
                }
            }

            s1 = Guid.Empty;
            return false;
        }

        public override void Init()
        {
            Request(_pageUrl, async (res) => await ParseData(res));
        }

        public new async Task Start(string pageUrl)
        {
            _pageUrl = pageUrl;
            await StartAsync();
        }
    }
}
