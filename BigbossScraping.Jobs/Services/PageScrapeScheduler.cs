using System;
using BigbossScraping.Contracts.Interfaces;
using Hangfire;

namespace BigbossScraping.Jobs.Services
{
    class PageScrapeScheduler : IPageScrapeScheduler
    {
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly IRecurringJobManager _recurringJobManager;

        public PageScrapeScheduler()
        {
            // _backgroundJobClient = backgroundJobClient;
            //_recurringJobManager = recurringJobManager;
        }

        public string ScheduleArticleJob(string pageUrl, string parentJob = null, string groupId = null)
        {
            if (string.IsNullOrEmpty(parentJob))
            {
                return _backgroundJobClient.Schedule<IArticleScrapper>(x => x.StartPageScraper(pageUrl, false, groupId),
                    TimeSpan.FromSeconds(10));
            }

            return _backgroundJobClient.ContinueJobWith<IArticleScrapper>(parentJob,
                x => x.StartPageScraper(pageUrl, false, groupId), JobContinuationOptions.OnAnyFinishedState);
        }

        public string SchedulePagedArticleJob(string pageUrl, string parentJob = null, string groupId = null)
        {
            if (string.IsNullOrEmpty(parentJob))
            {
                return _backgroundJobClient.Enqueue<IPagedArticleScrapper>(x => x.StartPageScraper(pageUrl, false, groupId));
            }

            return _backgroundJobClient.ContinueJobWith<IPagedArticleScrapper>(parentJob,
                x => x.StartPageScraper(pageUrl, false, groupId), JobContinuationOptions.OnAnyFinishedState);
        }

        public string ScheduleCategoryJob(string pageUrl, string parentJob = null, string groupId = null)
        {
            if (string.IsNullOrEmpty(parentJob))
            {
                return _backgroundJobClient.Enqueue<IProgramCategoryScrapper>(x => x.StartPageScraper(pageUrl, false, groupId));
            }

            return _backgroundJobClient.ContinueJobWith<IProgramCategoryScrapper>(parentJob,
                x => x.StartPageScraper(pageUrl, false, groupId), JobContinuationOptions.OnAnyFinishedState);
        }

        public string ScheduleProgramJob(string pageUrl, string parentJob = null, string groupId = null)
        {
            if (string.IsNullOrEmpty(parentJob))
            {
                return _backgroundJobClient.Enqueue<IProgrammeScrapper>(x => x.StartPageScraper(pageUrl, false, groupId));
            }

            return _backgroundJobClient.ContinueJobWith<IProgrammeScrapper>(parentJob,
                x => x.StartPageScraper(pageUrl, false, groupId), JobContinuationOptions.OnAnyFinishedState);
        }

        public string ScheduleParsingJobFromCategory(bool includeHome = false)
        {
            return _backgroundJobClient.Schedule<IScheduledSiteInformationService>(x => x.StartCategoryParsing(includeHome), TimeSpan.FromSeconds(10));
        }

        public string ScheduleHomeParsingJob(string pageUrl)
        {
            return _backgroundJobClient.Schedule<IHomePageScrapper>(x => x.Start(pageUrl), TimeSpan.FromMinutes(1));
        }

        public void RaisePageScrapeCompleted(string payload, ResponseType responseType)
        {
            _backgroundJobClient.Schedule<IScheduledSiteInformationService>(x =>
                x.AddResponseToStorage(new ResponsePayload { Response = payload, ResponseType = responseType }), TimeSpan.FromSeconds(1));
        }

        public void CreateRecurringJob(string url, int jobId)
        {
            _recurringJobManager.AddOrUpdate<IScheduledSiteInformationService>(jobId.ToString(), service => service.StartSiteParsing(url), Cron.MinuteInterval(15));
        }

        public void PurgeJobs()
        {
            JobStorage.Current?.GetMonitoringApi()?.PurgeJobs();
        }
    }
}
