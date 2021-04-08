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
    class ProgramDetailsScraper : WebScraper, IProgrammeScrapper
    {
        private readonly IPageScrapeScheduler _pageScrapeScheduler;
        private readonly ILogger<ProgramDetailsScraper> _logger;
        private string _pageUrl;
        private string _programId;
        private bool _onDemand;

        public ProgramDetailsScraper(IPageScrapeScheduler pageScrapeScheduler, ILogger<ProgramDetailsScraper> logger)
        {
            _pageScrapeScheduler = pageScrapeScheduler;
            _logger = logger;
            LoggingLevel = LogLevel.Critical;
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
            var videoUrls = new List<string>();
            string videoBanner = string.Empty;
            var res = response.Document.GetFirstArticleNode();
            if (res != null)
            {

                var itemTitle = res.GetFirstHeader1Node().GetCleanTextFromNode();
                var time = itemTitle.GetFirstDateFromString();
                
                if (res.GetIFrameNodes().Length > 1)
                {
                    foreach (var iFrameNode in res.GetIFrameNodes())
                    {
                        videoUrls.Add(iFrameNode.GetSourceAttribute());
                    }
                }

                var itemUrls = res.GetFirstIFrameNode().GetSourceAttribute()
                        .Split(new[] { "&img" }, StringSplitOptions.None);

                var itemContent = res.GetCleanTextFromNode();

                var videoUrl = new[] { itemUrls[0] };
                if (itemUrls.Length > 1)
                {
                    videoBanner = itemUrls[1];
                }

                var items = response.Document.GetRelatedPostNode();
                IList<ProgramInformation> programInformations = new List<ProgramInformation>();
                foreach (var htmlNode in items.First())
                {
                    var programInformation = new ProgramInformation
                    {
                        Url = htmlNode.GetFirstAnchorNode().GetHyperlinkReferenceFromAnchor(),
                        Image = htmlNode.GetFirstImageNode().GetSourceAttribute(),
                        ImageAlternative = htmlNode.GetFirstImageNode().GetAltFromImage(),
                        Title = htmlNode.GetFirstAnchorNode().GetCleanTextFromNode()
                    };

                    programInformations.Add(programInformation);
                }

                var responseProgrammeDetails = new ProgrammeDetails
                {
                    Title = itemTitle,
                    EpisodeDate = time.HasValue? time.Value.ToShortDateString() : DateTime.Now.ToShortDateString(),
                    Content = itemContent,
                    VideoBanner = videoBanner,
                    VideoUrl = videoUrls.Count > 1 ? videoUrls.ToArray() : videoUrl,
                    Id = Guid.NewGuid(),
                    ProgramId = string.IsNullOrEmpty(_programId) ? Guid.Empty : Guid.Parse(_programId),

                    ProgramInformations =  programInformations
                };

                _logger.LogInformation(
                    $"Video details of program {responseProgrammeDetails.Title} - for program Id : {responseProgrammeDetails.ProgramId} -- {responseProgrammeDetails.VideoUrl}");

                if (!_onDemand)
                {
                    await Task.Delay(TimeSpan.FromSeconds(1));

                    _pageScrapeScheduler.RaisePageScrapeCompleted(JsonConvert.SerializeObject(responseProgrammeDetails),
                        ResponseType.Programme);
                }
                else
                {
                    ParsedResponse(responseProgrammeDetails);
                }
            }
        }

        public Task StartPageScraper(string pageUrl, bool onDemand = false, string groupId = null)
        {
            _pageUrl = pageUrl;
            _programId = groupId;
            _onDemand = onDemand;

            Init();
            return StartAsync();
        }

        public Action<ProgrammeDetails> ParsedResponse { get; set; }
    }
}