using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BigbossScraping.Contracts.Interfaces;
using BigbossScraping.Contracts.Model;
using BigbossScraping.IronScraper.Extensions;
using IronWebScraper;
using Newtonsoft.Json;

namespace BigbossScraping.IronScraper.Services
{
    class ProgramCategoryScrapper : WebScraper, IProgramCategoryScrapper
    {
        private readonly IPageScrapeScheduler _pageScrapeScheduler;

        private string _pageUrl;
        private bool _onDemand;
        private readonly IDictionary<string, IList<Category>> _responseList;

        public ProgramCategoryScrapper(IPageScrapeScheduler pageScrapeScheduler)
        {
            _pageScrapeScheduler = pageScrapeScheduler;
            _responseList = new Dictionary<string, IList<Category>>();
            LoggingLevel = LogLevel.Critical;
        }

        private async Task ParseData(Response response)
        {
            foreach (var node in response.Document.GetElementById("menu-menu-1").ChildNodes
                .Where(x => !string.IsNullOrEmpty(x.GetCleanTextFromNode())).ToList())
            {
                var list = new List<Category>();
                if (node.GetElementsByTagName("ul").Any())
                {
                    foreach (var subMenu in node.GetElementsByTagName("li").ToList())
                    {
                        list.Add(new Category
                        {
                            Title = subMenu.GetFirstAnchorNode().GetCleanTextFromNode(),
                            Url = subMenu.GetFirstAnchorNode().GetHyperlinkReferenceFromAnchor(),
                        });
                    }
                }

                if (!list.Any())
                {
                    list.Add(new Category
                    {
                        Title = node.InnerTextClean,
                        Url = node.GetFirstAnchorNode().GetHyperlinkReferenceFromAnchor(),
                    });
                }

                _responseList.Add(node.GetFirstAnchorNode().InnerTextClean, list);
            }

            if (!_onDemand)
            {
                await Task.Delay(TimeSpan.FromSeconds(1));

                _pageScrapeScheduler.RaisePageScrapeCompleted(JsonConvert.SerializeObject(_responseList),
                    ResponseType.Category);
            }
            else
            {
                ParsedResponse(_responseList);
            }
        }

        public override void Parse(Response response)
        {

        }

        public Action<IDictionary<string, IList<Category>>> ParsedResponse { get; set; }

        public override void Init()
        {
            Request(_pageUrl, async (response) => await ParseData(response));
        }

        public Task StartPageScraper(string pageUrl, bool onDemand = false, string groupId = null)
        {
            _pageUrl = pageUrl;
            _onDemand = onDemand;

            return StartAsync();
        }
    }
}
